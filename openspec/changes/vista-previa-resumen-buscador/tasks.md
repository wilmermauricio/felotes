## 1. Layout y pantalla de carga

- [x] 1.1 Quitar el `<header>` de `_Layout.cshtml`, conservando la alerta de TempData
- [x] 1.2 Agregar en `Index.cshtml` un botón visible "Descargar plantilla"

## 2. ViewModel

- [x] 2.1 Agregar a `VistaPreviaViewModel` el resumen de la carga (TotalComprobantes, TotalLineas, Validos, ConErrores) y `ResumenPorTipo` (Tipo, Cantidad, ConErrores, MontoTotal de los válidos, "Desconocido" para tipo nulo, ordenado por código)
- [x] 2.2 Agregar a `ComprobanteVistaPrevia` `TextoBusqueda`, normalizado sin acentos y en minúsculas, con todas las propiedades de `EncabezadoFila` (por reflexión), Empresa, ENCF, tipo, líneas y errores

## 3. Eliminar la carga

- [x] 3.1 Agregar `POST Eliminar(Guid id)` con antiforgery en `ImportacionController`, que usa `almacen.Retirar`, pone el mensaje "Se eliminó la carga del archivo '<nombre>'" y redirige a Index (sin error si ya no existe)

## 4. Vista previa

- [x] 4.1 Reemplazar "Subir otro archivo" por el formulario "Eliminar carga y subir de nuevo", con `confirm()`
- [x] 4.2 Agregar la tarjeta de resumen de la carga y la tabla por tipo con bordes
- [x] 4.3 Reemplazar las tarjetas por una sola `table table-bordered` de comprobantes, con fila principal y fila de detalle (tabla anidada con bordes y errores). Las filas con errores quedan expandidas
- [x] 4.4 Agregar el campo de búsqueda, el contador "N de M comprobantes" y el mensaje "Ningún comprobante coincide con la búsqueda"

## 5. JavaScript y estilos

- [x] 5.1 En `site.js`: alternar la fila de detalle (▸/▾) y filtrar por `data-busqueda`, con normalización sin acentos, retardo de 150 ms y actualización del contador
- [x] 5.2 En `site.css`: separadores verticales visibles, encabezado de tabla destacado y cursor en las filas que se pueden expandir

## 6. Verificación

- [x] 6.1 `dotnet build` y `dotnet test` sin errores
- [x] 6.2 Prueba manual con `docs/ejemplo-todos-los-tipos.xlsx`:
  - el resumen muestra 11 comprobantes;
  - la tabla por tipo muestra 2 comprobantes del tipo 31;
  - la búsqueda "laptop" muestra 2 resultados (la factura y la nota de crédito);
  - la búsqueda "E310000000050" muestra la nota de débito;
  - la búsqueda "credito" encuentra los tipos con "Crédito";
  - eliminar la carga y luego confirmar responde "expiró"
- [x] 6.3 Actualizar el README (flujo: eliminar carga en lugar de subir otro archivo)
