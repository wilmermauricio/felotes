## MODIFIED Requirements

### Requirement: Subida y vista previa
Al subir un archivo válido, el sistema SHALL procesarlo completo y redirigir a una vista previa identificada por un `importId` único. La aplicación MUST NOT mostrar un header ni una barra de navegación. La vista previa SHALL mostrar:

- un resumen de la carga, con el total de comprobantes, el total de líneas de detalle, la cantidad de válidos y la de comprobantes con errores;
- una tabla de resumen por tipo de e-CF (código y descripción), con la cantidad de comprobantes, cuántos tienen errores y el monto total de los válidos. Los comprobantes cuyo tipo no se pudo deducir se agrupan como "Desconocido";
- una sola tabla de comprobantes con bordes y separadores entre columnas. Cada fila muestra estado, Empresa, ENCF, tipo de e-CF, fecha de emisión, comprador, cantidad de líneas, TotalITBIS y MontoTotal;
- los errores del archivo, de las filas y de los comprobantes, con el número de fila de Excel cuando aplique.

La vista previa MUST NOT ofrecer el botón "Subir otro archivo".

#### Scenario: Archivo con comprobantes válidos e inválidos
- **WHEN** el usuario sube un archivo con 3 comprobantes, de los cuales 1 tiene errores
- **THEN** el resumen muestra 3 comprobantes, 2 válidos y 1 con errores, y la fila del comprobante inválido aparece expandida con sus errores y sus filas de Excel

#### Scenario: Resumen por tipo de e-CF
- **WHEN** el archivo tiene dos comprobantes tipo 31 válidos (totales 100 y 200) y un comprobante tipo 32 con errores
- **THEN** la tabla de resumen muestra la fila "31 - Factura de Crédito Fiscal" con cantidad 2, 0 con errores y monto 300.00, y la fila "32 - Factura de Consumo" con cantidad 1 y 1 con errores

#### Scenario: Ver detalles de un comprobante
- **WHEN** el usuario expande un comprobante en la tabla
- **THEN** ve sus líneas de detalle en una tabla con bordes, con línea, fila de Excel, nombre, cantidad, precio unitario y monto

#### Scenario: Sin header
- **WHEN** se muestra cualquier pantalla de la aplicación
- **THEN** no aparece una barra de navegación, y el enlace "Descargar plantilla" está en la pantalla de carga

## ADDED Requirements

### Requirement: Buscador de comprobantes
La vista previa SHALL tener un campo de búsqueda que filtre la tabla de comprobantes mientras el usuario escribe, sin recargar la página. La búsqueda MUST:

- no distinguir mayúsculas, minúsculas ni acentos;
- comparar contra todos los campos de encabezado del comprobante (Empresa, ENCF, tipo, fechas, tipo de pago, tipo de ingreso, RNC o identificador del comprador, razón social, NCF modificado, código de modificación, moneda), sus líneas de detalle (nombre del ítem, cantidades y montos) y sus mensajes de error;
- mostrar el comprobante completo cuando coincide cualquiera de sus campos o líneas;
- mostrar el contador "N de M comprobantes".

Con el campo vacío, se muestran todos los comprobantes.

#### Scenario: Buscar por nombre de ítem
- **WHEN** el usuario escribe "laptop" y solo el comprobante E310000000001 de EMP01 tiene una línea "Laptop 14 pulgadas"
- **THEN** la tabla muestra solo ese comprobante y el contador indica "1 de 11 comprobantes"

#### Scenario: Buscar por un campo que no está en la tabla
- **WHEN** el usuario escribe "E310000000050", que es el NCFModificado de la nota de débito
- **THEN** la tabla muestra la nota de débito

#### Scenario: Búsqueda sin acentos
- **WHEN** el usuario escribe "credito"
- **THEN** aparecen los comprobantes cuyo tipo contiene "Crédito"

#### Scenario: Sin coincidencias
- **WHEN** ningún comprobante coincide con el texto
- **THEN** la tabla queda vacía, se muestra "Ningún comprobante coincide con la búsqueda" y el contador indica "0 de M comprobantes"

### Requirement: Eliminar la carga
La vista previa SHALL ofrecer la acción "Eliminar carga y subir de nuevo", tanto si la importación tiene errores como si no. Antes de ejecutarla, el sistema MUST pedir confirmación al usuario. Al confirmar, el sistema SHALL quitar la importación de la memoria y volver a la pantalla de carga con el mensaje "Se eliminó la carga del archivo '<nombre>'". La acción MUST usar POST con token antiforgery.

#### Scenario: Eliminar una carga con errores
- **WHEN** el usuario elimina una carga con errores y acepta la confirmación
- **THEN** la importación deja de existir, el usuario vuelve a la pantalla de carga con el mensaje de eliminación y puede subir el archivo corregido

#### Scenario: Confirmar después de eliminar
- **WHEN** el usuario elimina la carga y luego envía la confirmación de ese mismo `importId`
- **THEN** el sistema responde "La importación expiró, vuelva a subir el archivo" y no llama al repositorio

#### Scenario: Eliminar una carga que ya no existe
- **WHEN** el usuario elimina una carga vencida o ya eliminada
- **THEN** el sistema vuelve a la pantalla de carga sin error
