## ADDED Requirements

### Requirement: Columnas de la plantilla
La plantilla SHALL tener una sola hoja con una fila de encabezados y, como máximo, las siguientes columnas: `Empresa`, `ENCF`, `FechaEmision`, `FechaVencimientoSecuencia`, `TipoPago`, `TipoIngreso`, `FechaLimitePago`, `ArticulosIncluyenITBIS`, `RNCComprador`, `IdentificadorExtranjero`, `RazonSocialComprador`, `NCFModificado`, `FechaNCFModificado`, `CodigoModificacion`, `TipoMoneda`, `TipoCambio`, `NombreItem`, `BienServicio`, `TipoITBIS`, `Cantidad`, `PrecioUnitario`, `IndicadorAgenteRetencion`, `MontoITBISRetenido`, `MontoISRRetenido`. Las tres columnas de retención son opcionales y solo aplican a los tipos 41 y 47, cuyos detalles exigen un objeto `Retencion` en la librería. La plantilla MUST NOT incluir columnas de datos del emisor ni una columna de tipo de e-CF.

#### Scenario: Descarga de la plantilla
- **WHEN** el usuario solicita descargar la plantilla
- **THEN** el sistema entrega un archivo `.xlsx` con una hoja cuyos encabezados son exactamente las columnas listadas, en ese orden

#### Scenario: La plantilla no pide datos del emisor
- **WHEN** se inspeccionan los encabezados de la plantilla
- **THEN** no existe ninguna columna de RNC, razón social ni dirección del emisor

### Requirement: Localización de columnas por nombre
El lector SHALL localizar cada columna por el texto de su encabezado, sin distinguir mayúsculas ni espacios al inicio o al final, y no por su posición. Las columnas con encabezados desconocidos SHALL ignorarse. Si falta un encabezado obligatorio para todos los tipos (`Empresa`, `ENCF`, `FechaEmision`, `NombreItem`, `BienServicio`, `TipoITBIS`, `Cantidad`, `PrecioUnitario`), el archivo MUST rechazarse con un error que nombre la columna faltante.

#### Scenario: Columnas reordenadas
- **WHEN** el archivo tiene todas las columnas, pero en un orden distinto al de la plantilla
- **THEN** cada valor se lee en el campo correcto

#### Scenario: Columna obligatoria ausente
- **WHEN** el archivo no tiene la columna `ENCF`
- **THEN** el sistema rechaza el archivo con el mensaje "Falta la columna obligatoria 'ENCF'" y no procesa ninguna fila

#### Scenario: Columna opcional ausente
- **WHEN** el archivo no tiene la columna `TipoMoneda`
- **THEN** el sistema procesa el archivo tratando ese campo como vacío en todas las filas

### Requirement: Lectura de filas tipadas
El lector SHALL convertir cada fila no vacía en un registro con valores tipados (texto, entero, decimal, fecha, booleano) y conservar su número de fila de Excel. Las filas totalmente vacías SHALL ignorarse. Las fechas MUST aceptarse como celdas de fecha de Excel o como texto `dd-MM-yyyy` o `dd/MM/yyyy`. `ArticulosIncluyenITBIS` MUST aceptar `1`/`0`, `Si`/`No` y `true`/`false`.

#### Scenario: Valor con formato inválido
- **WHEN** la celda `Cantidad` de la fila 7 contiene "abc"
- **THEN** el sistema registra el error "Fila 7, columna 'Cantidad': 'abc' no es un número válido" y continúa leyendo las demás filas

#### Scenario: Fecha como texto
- **WHEN** la celda `FechaEmision` contiene el texto "20-11-2026"
- **THEN** el valor se interpreta como 20 de noviembre de 2026

#### Scenario: Fila vacía intermedia
- **WHEN** la fila 5 está completamente vacía entre filas con datos
- **THEN** la fila 5 se ignora sin generar errores

### Requirement: Validación del archivo subido
El sistema SHALL aceptar solo archivos `.xlsx` de hasta 10 MB. Un archivo sin filas de datos MUST rechazarse.

#### Scenario: Extensión no permitida
- **WHEN** el usuario sube un archivo `.csv`
- **THEN** el sistema muestra "Solo se aceptan archivos .xlsx" y no lo procesa

#### Scenario: Archivo sin datos
- **WHEN** el archivo solo contiene la fila de encabezados
- **THEN** el sistema muestra "El archivo no contiene filas de datos"
