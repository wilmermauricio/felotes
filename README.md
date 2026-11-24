# Importador de e-CF desde Excel

Aplicación ASP.NET Core MVC (.NET 10) para cargar comprobantes fiscales electrónicos (e-CF) de la DGII desde un archivo Excel, validarlos con la librería `DGII.FE.XML` y entregarlos a la capa de persistencia.

## Ejecutar

```bash
dotnet run --project src/FeExcel.Web
dotnet test
```

La solución referencia el proyecto `../Libraries/DGII.FE.XML` (y este, a su vez, `../Libraries/Utility.XML`). Ambos deben existir junto a la carpeta `FeExcel`. No hace falta ningún paso previo: `dotnet build` los compila.

## Flujo

1. **Descargar la plantilla** (botón en la pantalla de carga). Cada encabezado tiene una nota con los valores permitidos. En `docs/ejemplo-todos-los-tipos.xlsx` hay un ejemplo con un comprobante de cada tipo.
2. **Llenar una fila por línea de detalle.** Las filas con la misma `Empresa` y el mismo `ENCF` forman un comprobante. Los datos de encabezado deben repetirse iguales en todas sus filas.
3. **Subir el archivo** para ver la vista previa:
   - un resumen de la carga, con totales y un desglose por tipo de e-CF;
   - una tabla de comprobantes con totales calculados por la librería DGII, que se expande para ver las líneas y los errores;
   - un buscador sobre todos los campos (no distingue mayúsculas ni acentos).
4. **Confirmar** o **eliminar la carga y subir de nuevo.** Solo se puede confirmar si no hay ningún error (todo o nada). Si hay errores, elimine la carga, corrija el archivo y vuelva a subirlo. La importación se guarda en memoria durante 30 minutos.

Formatos de las celdas:
- Fechas: celda de fecha de Excel, o texto `dd-MM-yyyy` o `dd/MM/yyyy`.
- Números: celda numérica, o texto con punto decimal.
- Si/No: `Si`/`No`, `1`/`0` o `true`/`false`.

El tipo de e-CF se deduce del ENCF: `E31…` es Crédito Fiscal, `E32…` es Consumo, y así sucesivamente.

## Arquitectura

```
src/
  FeExcel.Domain          POCOs que recibe su repositorio (ComprobanteImportado, DetalleImportado, ...)
  FeExcel.Application     Casos de uso, reglas, adaptador de la librería DGII y puertos (interfaces)
  FeExcel.Infrastructure  ClosedXML, caché en memoria y stubs de emisor y repositorio
  FeExcel.Web             Controlador MVC y vistas
tests/
  FeExcel.Application.Tests
```

`Plantilla/ColumnasPlantilla.cs` es la única fuente de verdad de las columnas: nombres, orden, tipos y notas de ayuda.

## Lo que usted debe implementar

Los dos puntos se registran en `src/FeExcel.Infrastructure/DependencyInjection.cs`. Cambie el stub por su clase.

| Interfaz | Stub actual | Qué debe hacer |
|---|---|---|
| `IEmisorProvider` | `Emisores/EmisorProviderStub.cs`: devuelve un emisor genérico para cualquier empresa | Buscar el emisor de la empresa en su BD. Devuelva `null` si no existe y el comprobante quedará con error. |
| `IComprobanteRepository` | `Persistencia/ComprobanteRepositoryStub.cs`: solo escribe en el log | Mapear `ComprobanteImportado` y `DetalleImportado` a sus tablas, en una sola transacción. |

Los comentarios `TODO` de cada stub muestran qué propiedad va a qué lugar. `ComprobanteImportado` no depende de Excel ni de la librería DGII; los totales ya vienen calculados.
