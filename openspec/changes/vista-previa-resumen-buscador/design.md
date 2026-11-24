## Context

La vista previa (`VistaPrevia.cshtml`) muestra cada comprobante como una tarjeta `<details>`, con una tabla de detalle sin bordes, y un botón "Subir otro archivo". El layout tiene un header con el nombre de la app y "Descargar plantilla". El resultado de la importación vive en `IAlmacenImportaciones` (memoria, 30 min), que ya tiene `Retirar(id)` atómico. Todo el cambio está en la capa Web.

## Goals / Non-Goals

**Goals:**
- Resumen de la carga: totales y desglose por tipo de e-CF.
- Una sola tabla de comprobantes con bordes, con filas que se expanden.
- Buscador en el navegador sobre todos los campos.
- Eliminar la carga y volver a subir.
- Sin header.

**Non-Goals:**
- Paginación o búsqueda en el servidor. Los archivos esperados tienen cientos de comprobantes, no decenas de miles.
- Filtros adicionales (por tipo o "solo con errores"). Se pueden agregar después sobre el mismo mecanismo.
- Cambios en Application, Domain o Infrastructure.
- Pruebas automatizadas de la capa Web: no existe un proyecto de pruebas Web. Se verifica de forma manual.

## Decisions

### D1. El resumen se calcula en el ViewModel
`VistaPreviaViewModel` agrega un `ResumenCarga` (TotalComprobantes, TotalLineas, Validos, ConErrores) y una lista `ResumenPorTipo` (Tipo, Cantidad, ConErrores, MontoTotal), agrupada por `ComprobanteProcesado.Tipo`. Los comprobantes de tipo nulo forman la fila "Desconocido". Las filas se ordenan por código de tipo.
- *Alternativa:* calcularlo en `ResultadoImportacion` (Application). Se descarta porque es información de presentación y ningún otro consumidor la necesita.

### D2. Una tabla con fila de detalle expandible
Cada comprobante ocupa dos `<tr>` en una `table table-bordered`:
- la fila principal, con un botón ▸/▾;
- la fila de detalle, con `colspan` completo, que contiene los errores y una tabla anidada `table table-sm table-bordered` con las líneas.

El toggle es JavaScript mínimo, que alterna el atributo `hidden`. Los comprobantes con errores se muestran expandidos de entrada.
- *Alternativa:* el colapso de Bootstrap. Se descarta porque trae dependencias de animación sobre `<tr>` que se comportan mal con `colspan`. Con `hidden` basta.

### D3. Búsqueda con un atributo de texto precalculado
`ComprobanteVistaPrevia` agrega `TextoBusqueda`, que se arma en el servidor:
- todas las propiedades de `EncabezadoFila`, leídas por reflexión para que una columna nueva entre sola, con las fechas en `dd-MM-yyyy`;
- Empresa, ENCF y la descripción del tipo;
- el nombre, la cantidad, el precio y el monto de cada línea;
- los mensajes de error.

El texto se normaliza a minúsculas y sin acentos (`NormalizationForm.FormD`, quitando `NonSpacingMark`) y se escribe en `data-busqueda` de la fila principal. En el navegador, `site.js` normaliza lo que escribe el usuario de la misma forma y oculta los dos `<tr>` del comprobante si su texto no lo contiene. Un evento `input` con un retardo de 150 ms actualiza el contador "N de M comprobantes" y el mensaje "Ningún comprobante coincide…".
- *Alternativa:* recorrer el `textContent` de la fila. Se descarta porque no incluye los campos que no se muestran (NCFModificado, moneda, etc.) y mezcla etiquetas de la interfaz.
- *Alternativa:* búsqueda en el servidor. Se descarta porque los datos ya están en la página y una petición por tecla sobra.

### D4. Eliminar con POST y Retirar
Nueva acción `POST Importacion/Eliminar/{id}` con `[ValidateAntiForgeryToken]`. Llama a `almacen.Retirar(id)`; si devuelve la importación, el mensaje en TempData incluye el nombre del archivo. En todos los casos redirige a `Index`, así que eliminar dos veces no da error. El botón usa `onclick="return confirm(...)"`.
- *Alternativa:* agregar un método `Eliminar` al puerto. Se descarta porque `Retirar` ya hace exactamente eso.

### D5. Layout sin header
Se quita el `<header>` de `_Layout.cshtml`. Se conserva la alerta de TempData. "Descargar plantilla" pasa a ser un botón visible en `Index.cshtml`. `Resultado.cshtml` mantiene "Importar otro archivo", porque en ese punto la carga ya no existe.

## Risks / Trade-offs

- [Archivos con miles de comprobantes hacen la página pesada] → Aceptado. El límite de 10 MB acota el tamaño; si hace falta, se agrega paginación en otro cambio.
- [`data-busqueda` duplica el texto en el HTML] → Es poco texto por comprobante; se acepta a cambio de buscar en campos ocultos.
- [Este cambio modifica una capability que aún no está archivada] → Archivar `importador-comprobantes-excel` antes que este cambio.
