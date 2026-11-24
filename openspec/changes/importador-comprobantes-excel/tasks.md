## 1. Solución y proyectos

- [x] 1.1 Crear `FeExcel.slnx` y los proyectos `src/FeExcel.Domain`, `src/FeExcel.Application`, `src/FeExcel.Infrastructure` (classlib, net10.0), `src/FeExcel.Web` (mvc, net10.0) y `tests/FeExcel.Application.Tests` (xunit)
- [x] 1.2 Configurar las referencias: Application → Domain y `../Libraries/DGII.FE.XML/DGII.FE.XML/DGII.FE.XML.csproj`; Infrastructure → Application; Web → Application e Infrastructure; Tests → Application
- [x] 1.3 Agregar el paquete `ClosedXML` a Infrastructure y habilitar `Nullable` e `ImplicitUsings` en todos los proyectos
- [x] 1.4 Verificar que `dotnet build` compila la solución con la librería DGII (y documentar en el README cualquier paso previo)

## 2. Dominio

- [x] 2.1 Crear `TipoEcf` (valor 31 a 47, `Desde(encf)` con validación de formato y descripción legible)
- [x] 2.2 Crear `ComprobanteImportado`, `DetalleImportado`, `CompradorImportado`, `ReferenciaImportada` y `TotalesComprobante` como POCOs documentados para el mapeo a la BD
- [x] 2.3 Crear `ErrorImportacion` (Fila?, Campo?, Mensaje)

## 3. Aplicación: plantilla y abstracciones

- [x] 3.1 Crear `ColumnasPlantilla` (nombres y orden de columnas, y cuáles son obligatorias en el archivo)
- [x] 3.2 Crear `FilaComprobante` con `NumeroFila`, el `record EncabezadoFila` y los campos de detalle
- [x] 3.3 Definir los puertos `ILectorPlantilla`, `IGeneradorPlantilla`, `IEmisorProvider`, `IComprobanteRepository` e `IAlmacenImportaciones`

## 4. Aplicación: agrupación y reglas

- [x] 4.1 Implementar `AgrupadorComprobantes` (clave Empresa+ENCF normalizada, orden de primera aparición, numeración de líneas)
- [x] 4.2 Implementar `ValidadorConsistencia` (compara `EncabezadoFila` y reporta campo y filas en conflicto)
- [x] 4.3 Implementar `ReglasPorTipoEcf` (tabla de requeridos por tipo, campos de detalle por fila, moneda ambos-o-ninguno, RNC o identificador extranjero exclusivos)
- [x] 4.4 Pruebas unitarias de agrupación, consistencia y reglas por tipo (escenarios de `importacion-comprobantes`)

## 5. Aplicación: integración con la librería DGII

- [x] 5.1 Crear `IConstructorComprobante` y un helper compartido para construir comprador, referencia y detalles
- [x] 5.2 Implementar los constructores de los tipos 31, 32, 33 y 34
- [x] 5.3 Implementar los constructores de los tipos 41, 43, 44, 45, 46 y 47
- [x] 5.4 Implementar `ValidadorComprobantesDgii`: `FacturaFormatter` con `ServicioFacturaNoUsado`, traducción de excepciones y extracción de totales y `MontoItem`
- [x] 5.5 Pruebas: un comprobante válido por tipo produce totales, y un valor inválido (`BienServicio = 3`) produce un error legible

## 6. Aplicación: casos de uso

- [x] 6.1 Implementar `ProcesarArchivoHandler` (pipeline D2, acumulación de errores, guardado en `IAlmacenImportaciones`)
- [x] 6.2 Implementar `ConfirmarImportacionHandler` (todo o nada, quitar del almacén antes de llamar al repositorio, resultado con la cantidad enviada)
- [x] 6.3 Pruebas de los handlers con dobles de prueba (importación vencida, confirmar con errores, doble confirmación, emisor no encontrado)

## 7. Infraestructura

- [x] 7.1 Implementar `ConversorCeldas` (texto, entero, decimal, fecha de celda o texto, booleano) con mensajes por fila y columna
- [x] 7.2 Implementar `LectorPlantillaClosedXml` (encabezados por nombre, columnas obligatorias, filas vacías ignoradas)
- [x] 7.3 Implementar `GeneradorPlantillaClosedXml` (encabezados en negrita, fila congelada, formato de fecha)
- [x] 7.4 Implementar `AlmacenImportacionesMemoria` con `IMemoryCache` y TTL de 30 minutos
- [x] 7.5 Implementar `EmisorProviderStub` y `ComprobanteRepositoryStub` (con log) con comentarios TODO claros para el mapeo a la BD
- [x] 7.6 Crear `DependencyInjection.AddInfrastructure()` y `AddApplication()`

## 8. Web

- [x] 8.1 Configurar `Program.cs` (servicios, MemoryCache, límite de subida de 10 MB, ruta por defecto a Importacion)
- [x] 8.2 Implementar `ImportacionController` (Index, DescargarPlantilla, Subir, VistaPrevia, Confirmar) con antiforgery
- [x] 8.3 Crear los ViewModels y las vistas `Index`, `VistaPrevia` (resumen, tabla, errores, detalles expandibles, botón Confirmar condicional) y `Resultado`
- [x] 8.4 Validar extensión `.xlsx` y archivo vacío con mensajes del spec

## 9. Verificación

- [x] 9.1 `dotnet build` y `dotnet test` sin errores
- [x] 9.2 Prueba manual: descargar la plantilla, llenar un 31 con 2 detalles, un 32 y un 34 con un error, subir, revisar la vista previa, corregir y confirmar
- [x] 9.3 README breve: cómo ejecutar, formato de la plantilla y dónde implementar `IEmisorProvider` e `IComprobanteRepository`

## 10. ArticulosIncluyenITBIS explícito

- [x] 10.1 En `ReglasPorTipoEcf`, exigir `ArticulosIncluyenITBIS` en los tipos 31, 33, 34, 41 y 45 (el 32 ya lo exige)
- [x] 10.2 Actualizar la ayuda de la columna en `ColumnasPlantilla`: "Requerido en 31, 32, 33, 34, 41 y 45. Si = el precio ya incluye ITBIS; No = el ITBIS se suma al precio."
- [x] 10.3 Agregar pruebas: 31 sin `ArticulosIncluyenITBIS` da error; 43 sin la columna sigue siendo válido
- [x] 10.4 Regenerar `docs/ejemplo-todos-los-tipos.xlsx` con la plantilla nueva y verificar que sigue dando 11 válidos
