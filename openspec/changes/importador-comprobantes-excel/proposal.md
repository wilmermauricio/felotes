## Why

Hoy los comprobantes fiscales electrónicos (e-CF) se construyen por código usando los modelos de la librería `DGII.FE.XML`. No existe una forma de que un usuario cargue lotes de comprobantes desde Excel, validados con las mismas reglas de la librería, y que queden listos para que el equipo los persista en su base de datos con un mapeo hecho a mano.

## What Changes

- Nueva solución ASP.NET Core MVC en **.NET 10** con Clean Architecture: `FeExcel.Domain`, `FeExcel.Application`, `FeExcel.Infrastructure`, `FeExcel.Web` y un proyecto de pruebas.
- Plantilla Excel descargable (una sola hoja) con solo las columnas mínimas que exigen los modelos de la librería. Las columnas que no aplican a un tipo de e-CF se dejan en blanco.
- Lectura del Excel con **ClosedXML**. Cada fila es una línea de detalle; las filas con la misma **Empresa** y el mismo **ENCF** forman un solo comprobante.
- El tipo de e-CF (31, 32, 33, 34, 41, 43, 44, 45, 46, 47) se deduce del ENCF; no es una columna.
- Validación en tres niveles:
  - formato de celdas;
  - consistencia del encabezado dentro de cada grupo Empresa+ENCF;
  - reglas de la librería `DGII.FE.XML`, construyendo los modelos `Externo` y llamando a `FacturaFormatter.GetFacturaInterna`.
- Flujo **subir → vista previa → confirmar**. La vista previa muestra los comprobantes, sus totales calculados por la librería y los errores por fila o comprobante.
- Los datos del emisor no vienen en el Excel. Se resuelven por Empresa mediante el puerto `IEmisorProvider`, con un stub listo para conectar a la base de datos del usuario.
- Sin base de datos. La persistencia queda detrás del puerto `IComprobanteRepository`, con un stub y POCOs de dominio claros para que el usuario haga su mapeo manual.
- La carpeta `FeExcel/Models` (una copia desactualizada de la librería) **no** se usa; la solución referencia el proyecto `DGII.FE.XML` original.

## Capabilities

### New Capabilities
- `plantilla-excel-comprobantes`: estructura de la plantilla (columnas, formatos), su descarga y la lectura del archivo a filas tipadas.
- `importacion-comprobantes`: agrupación de filas en comprobantes por Empresa+ENCF, deducción del tipo de e-CF, detección de inconsistencias y validación con las reglas de la librería DGII.
- `vista-previa-confirmacion`: flujo web de subir, previsualizar con errores y totales, y confirmar el envío al repositorio de persistencia.

### Modified Capabilities
<!-- Ninguna: no existen specs previos. -->

## Impact

- **Código nuevo**: solución `FeExcel` con cuatro proyectos y pruebas bajo `FeExcel/src` y `FeExcel/tests`.
- **Dependencias**:
  - `ClosedXML` (MIT).
  - `ProjectReference` a `../Libraries/DGII.FE.XML/DGII.FE.XML/DGII.FE.XML.csproj` (net10.0), que a su vez depende del proyecto `Libraries/Utility.XML`.
  - Para las pruebas: `xUnit`.
- **Puntos de extensión para el usuario**: `IEmisorProvider` (buscar el emisor por empresa) e `IComprobanteRepository` (mapeo a la BD). Ambos se entregan como stubs con TODO.
- **Sin cambios** en la librería `DGII.FE.XML`.
