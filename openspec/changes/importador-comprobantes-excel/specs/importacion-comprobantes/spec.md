## ADDED Requirements

### Requirement: Agrupación por Empresa y ENCF
El sistema SHALL agrupar las filas que comparten `Empresa` y `ENCF` (comparación sin distinguir mayúsculas y sin espacios al inicio o al final) en un solo comprobante. Cada fila del grupo SHALL convertirse en una línea de detalle, numerada desde 1 en el orden en que aparece en el archivo.

#### Scenario: Comprobante con varios detalles
- **WHEN** las filas 2 y 3 tienen Empresa "EMP01" y ENCF "E310000000001"
- **THEN** se genera un único comprobante con dos líneas de detalle: la 1 tomada de la fila 2 y la 2 tomada de la fila 3

#### Scenario: Mismo ENCF en empresas distintas
- **WHEN** la fila 2 tiene Empresa "EMP01" y la fila 3 tiene Empresa "EMP02", ambas con ENCF "E310000000001"
- **THEN** se generan dos comprobantes distintos, uno por empresa

#### Scenario: Filas del grupo no contiguas
- **WHEN** las filas 2 y 9 comparten Empresa y ENCF, y entre ellas hay filas de otros comprobantes
- **THEN** ambas filas pertenecen al mismo comprobante

### Requirement: Consistencia del encabezado dentro del grupo
Todos los campos de encabezado (todas las columnas excepto `NombreItem`, `BienServicio`, `TipoITBIS`, `Cantidad` y `PrecioUnitario`) MUST tener el mismo valor en todas las filas de un grupo. Una diferencia MUST registrarse como error del comprobante, indicando el campo y las filas en conflicto.

#### Scenario: Fecha de emisión distinta en el mismo comprobante
- **WHEN** las filas 2 y 3 son del mismo grupo, pero la fila 2 tiene FechaEmision 20-11-2026 y la fila 3 tiene 21-11-2026
- **THEN** el comprobante queda marcado como inválido con el error "Inconsistencia en 'FechaEmision': las filas 2 y 3 tienen valores distintos"

#### Scenario: Encabezado repetido igual
- **WHEN** todas las filas del grupo repiten los mismos valores de encabezado
- **THEN** no se genera ningún error de consistencia

### Requirement: Deducción del tipo de e-CF
El sistema SHALL deducir el tipo de e-CF de los caracteres 2 y 3 del ENCF. El ENCF MUST tener 13 caracteres, empezar con "E" y su tipo MUST ser uno de 31, 32, 33, 34, 41, 43, 44, 45, 46 o 47.

#### Scenario: ENCF de crédito fiscal
- **WHEN** el ENCF es "E310000000001"
- **THEN** el comprobante se procesa como Factura de Crédito Fiscal Electrónica (tipo 31)

#### Scenario: Tipo no soportado
- **WHEN** el ENCF es "E990000000001"
- **THEN** el comprobante queda marcado como inválido con el error "Tipo de e-CF '99' no soportado"

### Requirement: Campos requeridos por tipo de e-CF
El sistema SHALL validar, antes de construir el modelo de la librería, que estén presentes los campos que cada tipo exige:

| Tipo | Campos requeridos además de Empresa, ENCF, FechaEmision y el detalle |
|---|---|
| 31 | TipoPago, TipoIngreso, FechaVencimientoSecuencia, RNCComprador, ArticulosIncluyenITBIS |
| 32 | TipoPago, TipoIngreso, ArticulosIncluyenITBIS |
| 33 | TipoPago, TipoIngreso, FechaVencimientoSecuencia, NCFModificado, FechaNCFModificado, CodigoModificacion, ArticulosIncluyenITBIS |
| 34 | TipoPago, TipoIngreso, NCFModificado, FechaNCFModificado, CodigoModificacion, ArticulosIncluyenITBIS |
| 41 | TipoPago, FechaVencimientoSecuencia, ArticulosIncluyenITBIS |
| 43 | FechaVencimientoSecuencia |
| 44 | TipoPago, FechaVencimientoSecuencia |
| 45 | TipoPago, FechaVencimientoSecuencia, ArticulosIncluyenITBIS |
| 46 | TipoPago, TipoIngreso, FechaVencimientoSecuencia |
| 47 | TipoPago, FechaVencimientoSecuencia |

La DGII exige `FechaVencimientoSecuencia` en todos los tipos excepto 32 y 34.

`ArticulosIncluyenITBIS` es requerido en todos los tipos cuyo modelo de la librería tiene esa propiedad (31, 32, 33, 34, 41 y 45). Cuando viene vacía, la librería trata el precio como si ya incluyera el ITBIS. Al exigirla, quien llena el archivo debe decidir de forma explícita cómo se calcula el ITBIS. En 43, 44, 46 y 47 no aplica y se ignora.

Las columnas de detalle `NombreItem`, `BienServicio`, `TipoITBIS`, `Cantidad` y `PrecioUnitario` MUST estar presentes en cada fila. `TipoMoneda` y `TipoCambio` MUST venir ambos o ninguno. `RNCComprador` e `IdentificadorExtranjero` MUST NOT venir juntos.

#### Scenario: Crédito fiscal sin RNC del comprador
- **WHEN** un comprobante tipo 31 no tiene `RNCComprador`
- **THEN** queda marcado como inválido con el error "El campo 'RNCComprador' es requerido para el tipo 31"

#### Scenario: Crédito fiscal sin indicar si el precio incluye ITBIS
- **WHEN** un comprobante tipo 31 tiene `ArticulosIncluyenITBIS` vacío
- **THEN** queda marcado como inválido con el error "El campo 'ArticulosIncluyenITBIS' es requerido para el tipo 31"

#### Scenario: Moneda sin tasa
- **WHEN** un comprobante tiene `TipoMoneda` "USD" y `TipoCambio` vacío
- **THEN** queda marcado como inválido con el error "Si se indica 'TipoMoneda' también debe indicarse 'TipoCambio'"

#### Scenario: Columna que no aplica
- **WHEN** un comprobante tipo 43 trae un valor en `RNCComprador`
- **THEN** el valor se ignora sin generar error

### Requirement: Validación con las reglas de la librería DGII
Para cada comprobante que supere las validaciones anteriores, el sistema SHALL construir el modelo `Externo` correspondiente de `DGII.FE.XML` (por ejemplo, `FacturaCreditoFiscalElectronica` para el tipo 31), con un emisor obtenido de `IEmisorProvider` según la Empresa. Luego SHALL obtener la `FacturaInterna` mediante `FacturaFormatter.GetFacturaInterna` y calcular sus totales. Toda excepción lanzada por la librería MUST convertirse en un error legible del comprobante; el proceso MUST continuar con los demás comprobantes.

#### Scenario: Comprobante válido
- **WHEN** un comprobante tipo 32 con dos detalles supera las reglas de la librería
- **THEN** queda marcado como válido, con MontoGravadoTotal, TotalITBIS y MontoTotal tomados de la `FacturaInterna`

#### Scenario: La librería rechaza un valor
- **WHEN** la librería lanza una excepción porque `BienServicio` vale 3
- **THEN** el comprobante queda inválido con un error que incluye el mensaje de la librería y la fila de origen cuando se puede determinar, y los demás comprobantes se siguen procesando

#### Scenario: Emisor no encontrado
- **WHEN** `IEmisorProvider` no devuelve un emisor para la Empresa "EMP99"
- **THEN** todos los comprobantes de "EMP99" quedan inválidos con el error "No se encontró el emisor para la empresa 'EMP99'"

### Requirement: Modelo de dominio para persistencia
El resultado SHALL exponerse como objetos de dominio sin dependencias de Excel, de la librería DGII ni de la base de datos:

- `ComprobanteImportado`: Empresa, ENCF, TipoEcf, fechas, pagos, ingreso, moneda, `CompradorImportado` opcional, `ReferenciaImportada` opcional, lista de `DetalleImportado` y totales calculados.
- `DetalleImportado`: número de línea, fila de origen, nombre, bien/servicio, tipo ITBIS, cantidad, precio unitario y monto calculado.

#### Scenario: Totales disponibles para el mapeo
- **WHEN** un comprobante es válido
- **THEN** su `ComprobanteImportado` incluye MontoGravadoTotal, TotalITBIS y MontoTotal, y cada `DetalleImportado` incluye su MontoItem, sin que quien lo consume tenga que referenciar la librería DGII
