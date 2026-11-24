## Why

Con archivos de muchos comprobantes, la vista previa actual es difícil de revisar. Muestra una lista de tarjetas sin un resumen por tipo, sin forma de buscar y con tablas sin bordes. Además, el header y el botón "Subir otro archivo" no aportan en esta pantalla. Lo que el usuario necesita es descartar la carga actual y volver a subir el archivo corregido.

## What Changes

- Quitar el header (barra de navegación) del layout. El enlace "Descargar plantilla" queda en la pantalla de carga.
- **BREAKING (UI):** quitar el botón "Subir otro archivo" de la vista previa. Lo reemplaza "Eliminar carga y subir de nuevo", que borra la importación de memoria y vuelve a la pantalla de carga.
- Agregar un resumen de la carga:
  - total de comprobantes, líneas, válidos y con errores;
  - una tabla por tipo de e-CF con cantidad de comprobantes, cantidad con errores y monto total.
- Reemplazar las tarjetas por una sola tabla de comprobantes, con bordes y separadores entre columnas. Cada fila se expande para mostrar sus líneas de detalle (también con bordes) y sus errores.
- Agregar un buscador que filtra los comprobantes en el navegador. Busca en todos los campos del encabezado, en las líneas de detalle y en los mensajes de error, y muestra el contador "N de M".

## Capabilities

### New Capabilities
<!-- Ninguna -->

### Modified Capabilities
- `vista-previa-confirmacion`: la vista previa agrega el resumen por tipo, la tabla con bordes, el buscador y la acción de eliminar la carga; se quita "Subir otro archivo".

## Impact

- `src/FeExcel.Web`: `_Layout.cshtml`, `Views/Importacion/{Index,VistaPrevia}.cshtml`, `Models/VistaPreviaViewModel.cs`, `ImportacionController` (nueva acción `Eliminar`), `wwwroot/js/site.js` y `wwwroot/css/site.css`.
- Se reutiliza `IAlmacenImportaciones.Retirar` para eliminar la carga. No cambian Application, Domain ni Infrastructure.
- Sin dependencias nuevas: el buscador es JavaScript sin librerías.
- Depende del cambio `importador-comprobantes-excel`, que define la capability `vista-previa-confirmacion`. Ese cambio debe archivarse antes que este.
