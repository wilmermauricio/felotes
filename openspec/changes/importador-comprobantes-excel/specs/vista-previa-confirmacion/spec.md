## ADDED Requirements

### Requirement: Subida y vista previa
Al subir un archivo válido, el sistema SHALL procesarlo completo y redirigir a una vista previa identificada por un `importId` único. La vista previa SHALL mostrar:

- un resumen: total de comprobantes, válidos e inválidos;
- por cada comprobante: Empresa, ENCF, tipo de e-CF, fecha de emisión, comprador, cantidad de detalles, MontoTotal y su estado;
- los errores del archivo, de las filas y de los comprobantes, con el número de fila de Excel cuando aplique.

#### Scenario: Archivo con comprobantes válidos e inválidos
- **WHEN** el usuario sube un archivo con 3 comprobantes, de los cuales 1 tiene errores
- **THEN** la vista previa muestra "3 comprobantes: 2 válidos, 1 con errores" y lista los errores del comprobante inválido junto a sus filas

#### Scenario: Ver detalles de un comprobante
- **WHEN** el usuario expande un comprobante en la vista previa
- **THEN** ve sus líneas de detalle con nombre, cantidad, precio unitario y monto

### Requirement: Resultado temporal sin base de datos
El resultado del procesamiento SHALL guardarse en memoria del servidor, asociado al `importId`, durante 30 minutos. Un `importId` inexistente o vencido MUST llevar al usuario de vuelta a la pantalla de carga con el mensaje "La importación expiró, vuelva a subir el archivo".

#### Scenario: Importación vencida
- **WHEN** el usuario confirma una importación procesada hace más de 30 minutos
- **THEN** el sistema muestra "La importación expiró, vuelva a subir el archivo" y no llama al repositorio

### Requirement: Confirmación
La acción Confirmar SHALL estar disponible solo cuando la importación no tiene ningún error. Al confirmar, el sistema SHALL entregar todos los `ComprobanteImportado` a `IComprobanteRepository` en una sola llamada, eliminar la importación de memoria y mostrar cuántos comprobantes se enviaron.

#### Scenario: Confirmación exitosa
- **WHEN** el usuario confirma una importación de 2 comprobantes sin errores
- **THEN** `IComprobanteRepository` recibe los 2 comprobantes y se muestra "Se enviaron 2 comprobantes"

#### Scenario: Confirmar con errores
- **WHEN** la importación tiene al menos un error
- **THEN** el botón Confirmar no aparece, y una petición directa de confirmación se rechaza con el mensaje "Corrija los errores y vuelva a subir el archivo"

#### Scenario: Doble confirmación
- **WHEN** el usuario envía dos veces la confirmación de la misma importación
- **THEN** el repositorio recibe los comprobantes una sola vez, y la segunda petición muestra que la importación ya no existe

### Requirement: Puertos de extensión con stubs
El sistema SHALL definir en la capa de aplicación los puertos `IEmisorProvider` e `IComprobanteRepository`, y entregar en infraestructura implementaciones stub registradas en el contenedor de dependencias:

- el stub de emisor devuelve un emisor genérico de validación para cualquier empresa;
- el stub de repositorio registra en el log la cantidad de comprobantes recibidos y no persiste nada.

Ambos stubs MUST contener comentarios TODO que indiquen dónde conectar la base de datos.

#### Scenario: Ejecución sin base de datos
- **WHEN** la aplicación se ejecuta con los stubs por defecto y el usuario confirma una importación válida
- **THEN** el flujo termina con éxito y el log registra la cantidad de comprobantes recibidos
