## Context

- La librería `Libraries/DGII.FE.XML` (net10.0) define los modelos `Externo` de cada e-CF. Expone `FacturaFormatter.GetFacturaInterna(Comprobante)` como método público, que valida y calcula los totales (ITBIS, MontoTotal).
- Sus reglas viven en setters que lanzan excepciones (por ejemplo, `TipoPago` null o `BienServicio` distinto de 1 o 2) y en conversiones `(int)` dentro de `ToFacturaInterna`.
- Varios modelos tienen huecos: el 31 no valida `FechaVencimientoSecuencia` (un `DateTime` no nullable), y las notas 33 y 34 no exigen `InformacionReferencia`.
- `FacturaFormatter` exige un `IFacturaService` en su constructor, aunque `GetFacturaInterna` no lo usa.
- `FeExcel/Models` es una copia desactualizada de la librería; se ignora.
- El usuario hará a mano el mapeo a su BD (los nombres de campos son distintos) y resolverá el emisor por empresa.

## Goals / Non-Goals

**Goals:**
- Importar e-CF desde un Excel de una sola hoja con las columnas mínimas.
- Validar con las reglas reales de la librería y mostrar los totales calculados en una vista previa.
- Dejar dos puntos de extensión obvios y aislados: `IEmisorProvider` e `IComprobanteRepository`.
- Código legible: clases pequeñas, nombres en español del dominio, sin lógica en los controladores.

**Non-Goals:**
- Persistencia real, autenticación, firma o envío a la DGII.
- Campos opcionales avanzados: descuentos o recargos, impuestos adicionales, transporte, informaciones adicionales de exportación, formas de pago múltiples, otra moneda por detalle. (Las retenciones sí se incluyen: ver D4.)
- Modificar la librería `DGII.FE.XML`.

## Decisions

### D1. Estructura de la solución (Clean Architecture)

```
FeExcel/
├── FeExcel.slnx
├── src/
│   ├── FeExcel.Domain/          (sin dependencias)
│   │   ├── Comprobantes/  ComprobanteImportado, DetalleImportado, CompradorImportado,
│   │   │                  ReferenciaImportada, TotalesComprobante, TipoEcf
│   │   └── Errores/       ErrorImportacion (Fila?, Campo?, Mensaje)
│   ├── FeExcel.Application/     (→ Domain, → DGII.FE.XML)
│   │   ├── Abstracciones/ ILectorPlantilla, IGeneradorPlantilla, IEmisorProvider,
│   │   │                  IComprobanteRepository, IAlmacenImportaciones, IValidadorDgii
│   │   ├── Plantilla/     FilaComprobante (fila tipada), ColumnasPlantilla (nombres)
│   │   ├── Importacion/   ProcesarArchivoHandler, ConfirmarImportacionHandler,
│   │   │                  AgrupadorComprobantes, ValidadorConsistencia,
│   │   │                  ReglasPorTipoEcf, ResultadoImportacion
│   │   └── Dgii/          ConstructorComprobanteDgii (+ uno por tipo), ValidadorComprobantesDgii
│   ├── FeExcel.Infrastructure/  (→ Application, ClosedXML)
│   │   ├── Excel/         LectorPlantillaClosedXml, GeneradorPlantillaClosedXml, ConversorCeldas
│   │   ├── Emisores/      EmisorProviderStub        ← TODO del usuario
│   │   ├── Persistencia/  ComprobanteRepositoryStub ← TODO del usuario
│   │   ├── Cache/         AlmacenImportacionesMemoria (IMemoryCache)
│   │   └── DependencyInjection.cs
│   └── FeExcel.Web/             (MVC, → Application, → Infrastructure)
│       ├── Controllers/   ImportacionController
│       ├── Models/        ViewModels
│       └── Views/Importacion/ Index, VistaPrevia, Resultado
└── tests/
    └── FeExcel.Application.Tests/  (xUnit)
```

`.slnx` es el formato por defecto del SDK de .NET 10.

**¿Por qué Application referencia `DGII.FE.XML`?** La validación con la librería es una regla de negocio central, no un detalle de infraestructura. La alternativa era esconderla detrás de un puerto en Infrastructure. Se descarta porque solo añade indirección: la librería es pura (no hace I/O). La concesión es que `IValidadorDgii` sigue siendo una interfaz, para poder simularla en pruebas.

**¿Por qué Domain no referencia la librería?** El mapeo del usuario a su BD solo debe depender de POCOs propios.

### D2. Pipeline de procesamiento

```
IFormFile ─▶ ILectorPlantilla.Leer(stream)
               └─▶ LecturaPlantilla { Filas: FilaComprobante[], Errores }   (errores de formato por celda)
         ─▶ AgrupadorComprobantes.Agrupar(filas)   clave: (Empresa, ENCF) normalizados
               └─▶ GrupoFilas[]  (orden de primera aparición)
         ─▶ por grupo:
               1. TipoEcf.Desde(encf)                 → error si es inválido
               2. ValidadorConsistencia               → un error por campo distinto
               3. ReglasPorTipoEcf.Validar(tipo, enc) → requeridos (tabla del spec)
               4. IEmisorProvider.ObtenerPorEmpresa
               5. ConstructorComprobanteDgii.Construir(tipo, grupo, emisor) → Comprobante (librería)
               6. ValidadorComprobantesDgii.Validar(comprobante) → FacturaInterna + Total, o error
               7. Mapear a ComprobanteImportado (+ totales)
         ─▶ ResultadoImportacion { Id, Comprobantes (con estado y errores), ErroresArchivo }
         ─▶ IAlmacenImportaciones.Guardar(resultado)  (TTL 30 min)
```

Los pasos 1 a 4 cortan el procesamiento del grupo al primer bloque con errores; los errores de un mismo paso se acumulan. Así no se pasan datos incompletos a la librería, cuyos mensajes son menos claros.

### D3. Una fila tipada única, y no un modelo por tipo

`FilaComprobante` tiene una propiedad nullable por columna, más `NumeroFila`. Los campos de encabezado se agrupan en un `record EncabezadoFila` para que `ValidadorConsistencia` compare con igualdad de records y reporte el campo exacto por reflexión sobre sus propiedades. La alternativa de comparar campo por campo a mano es repetitiva y frágil al agregar columnas.

### D4. Construcción de los modelos de la librería: un constructor por tipo

`IConstructorComprobante` tiene diez implementaciones, por ejemplo `ConstructorFacturaCreditoFiscal`, todas registradas en un diccionario `TipoEcf → constructor`.

- Usan el **constructor sin parámetros + inicializador de propiedades**. Los constructores de la librería tienen de 15 a 20 parámetros posicionales e inconsistencias (el 31 y el 32 no reciben `detalles`).
- Asignan `TipoMoneda`/`TipoCambio` **solo cuando ambos vienen**, porque sus setters se validan entre sí y dependen del orden. La regla "ambos o ninguno" ya se verificó en `ReglasPorTipoEcf`.
- `CompradorNacional` o `CompradorExtranjero` se elige según qué columna venga.
- La `InformacionReferencia` se arma solo para 33 y 34.
- Un helper común, `ConstruccionDgii.Detalles(...)`, evita repetir el mapeo de las cinco columnas de detalle. Recibe una función que crea el detalle, porque en los tipos 41 y 47 la librería declara `Retencion` como miembro `required`. Por eso la plantilla incluye tres columnas opcionales de retención (`IndicadorAgenteRetencion`, `MontoITBISRetenido`, `MontoISRRetenido`).
- La Nota de Crédito deduce `IndicadorNotaCredito` de las fechas: vale 1 si se emite más de 30 días después del comprobante modificado. Así no hace falta otra columna.
- Las propiedades opcionales cuyo setter rechaza `null` (por ejemplo, `TipoIngreso` en el tipo 45) se asignan solo si vienen.
- Se usa la librería vigente `Libraries/DGII.FE.XML`. La copia de `Certificacion/` es anterior y no calcula los totales.

Si un setter lanza una excepción, el constructor la deja propagarse y `ValidadorComprobantesDgii` la traduce a un error (D5).

### D5. Traducción de las excepciones de la librería

`ValidadorComprobantesDgii` envuelve la construcción y `GetFacturaInterna`:
- captura `ArgumentException`, `InvalidOperationException` y `NullReferenceException`, y genera `ErrorImportacion(Campo: ex.ParamName, Mensaje: ex.Message)`;
- fuerza el cálculo leyendo `facturaInterna.Total`;
- toma `MontoItem` de `facturaInterna.Detalles[i]` para cada detalle.

`FacturaFormatter` se instancia con un `ServicioFacturaNoUsado : IFacturaService` que lanza `NotSupportedException`. Esa clase queda privada del adaptador y documenta por qué existe.

### D6. Estado entre requests: IMemoryCache

`IAlmacenImportaciones` (Guardar / Obtener / Quitar) se implementa con `IMemoryCache`, con expiración absoluta de 30 minutos y un `Guid` como `importId`. `Quitar` se hace antes de llamar al repositorio, para que una doble confirmación no envíe dos veces.

Alternativas descartadas:
- `TempData`/cookies: tamaño limitado.
- Volver a subir el archivo: mala experiencia.
- Sesión: requiere configuración extra sin beneficio.

Limitación: un solo servidor; ver Riesgos.

### D7. Lectura con ClosedXML

- Encabezados en la fila 1, localizados por nombre normalizado (`Trim` + `OrdinalIgnoreCase`).
- `ConversorCeldas` centraliza la conversión de texto, entero, decimal, fecha (celda de fecha o texto `dd-MM-yyyy`/`dd/MM/yyyy`, cultura invariante) y booleano (`1/0`, `Si/No`, `true/false`).
- Cada fallo genera un `ErrorImportacion` con fila y columna, sin detener la lectura.
- La plantilla se genera con el mismo `ColumnasPlantilla`, la única fuente de verdad de nombres y orden, con encabezados en negrita, fila congelada y formato de fecha en las columnas de fecha.

### D8. Web

`ImportacionController` es delgado y tiene cinco acciones:

| Acción | Ruta | Qué hace |
|---|---|---|
| `Index` | GET | Formulario de carga y enlace a la plantilla |
| `DescargarPlantilla` | GET | Devuelve el `.xlsx` |
| `Subir` | POST, con antiforgery | Valida extensión y tamaño, llama a `ProcesarArchivoHandler` y redirige a `VistaPrevia/{id}` |
| `VistaPrevia/{id}` | GET | Muestra el resultado |
| `Confirmar/{id}` | POST, con antiforgery | Llama a `ConfirmarImportacionHandler` y muestra el resultado |

- Los ViewModels se construyen desde `ResultadoImportacion`; las vistas no tocan el dominio directamente.
- Bootstrap, que viene con la plantilla MVC.
- Los detalles se expanden con `<details>` de HTML, sin JS extra.

### D9. Dónde hace el usuario su trabajo manual

```csharp
// Infrastructure/Emisores/EmisorProviderStub.cs
// TODO: reemplazar por la consulta a su BD: Empresa → Emisor(RNCEmisor, RazonSocialEmisor, ...)

// Infrastructure/Persistencia/ComprobanteRepositoryStub.cs
// TODO: mapear ComprobanteImportado / DetalleImportado a sus tablas
```

Para conectarlos basta con cambiar una línea de registro en `DependencyInjection.cs`.

## Risks / Trade-offs

- **[Riesgo] Los mensajes de la librería son técnicos o en inglés** (por ejemplo, `NullReferenceException`). → `ReglasPorTipoEcf` atrapa antes los casos comunes con mensajes claros; el resto se muestra con el nombre del campo cuando existe.
- **[Riesgo] La librería es ofuscada** (`dotnet_reactor.targets`) **y depende de `Libraries/Utility.XML`.** → La tarea 1 verifica que la solución compile; si falla, se documenta el paso previo necesario.
- **[Trade-off] Errores tempranos por bloques.** Un grupo con un campo faltante no muestra también los errores que daría la librería. Esto es aceptable: al corregirlo y volver a subir aparecen.
- **[Riesgo] Excel con miles de filas.** → El límite de 10 MB y el procesamiento en memoria bastan para el uso esperado; no se optimiza más.
- **[Limitación] IMemoryCache no funciona con varias instancias del servidor.** → Detrás de `IAlmacenImportaciones`; se puede cambiar a `IDistributedCache` sin tocar Application.
- **[Riesgo] Reglas de la tabla por tipo distintas a las de la DGII real.** → La tabla vive en un solo lugar (`ReglasPorTipoEcf`) y es fácil de ajustar.

## Open Questions

- ¿Se debe permitir confirmar solo los comprobantes válidos cuando hay errores? Este diseño asume "todo o nada" (spec `vista-previa-confirmacion`); cambiarlo afecta solo a `ConfirmarImportacionHandler` y a la vista.
- ¿Se requiere detectar ENCF ya existentes en la BD? Queda fuera del alcance; el usuario puede hacerlo dentro de su repositorio.
