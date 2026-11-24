namespace FeExcel.Application.Plantilla;

public enum TipoDato
{
    Texto,
    Entero,
    Decimal,
    Fecha,
    Booleano
}

/// <param name="Nombre">Texto del encabezado en el Excel.</param>
/// <param name="Tipo">Tipo de dato esperado en la celda.</param>
/// <param name="ObligatoriaEnArchivo">Si la columna debe existir en el archivo (aunque algunas celdas puedan quedar vacías).</param>
/// <param name="Ayuda">Texto de ayuda que se muestra como nota en la plantilla.</param>
public sealed record ColumnaPlantilla(string Nombre, TipoDato Tipo, bool ObligatoriaEnArchivo, string Ayuda);

/// <summary>
/// Única fuente de verdad de las columnas de la plantilla: nombres, orden y tipos.
/// La usan tanto el generador de la plantilla como el lector.
/// </summary>
public static class ColumnasPlantilla
{
    public static readonly ColumnaPlantilla Empresa =
        new(nameof(FilaComprobante.Empresa), TipoDato.Texto, true, "Código de la empresa emisora. Se usa para buscar los datos del emisor.");

    public static readonly ColumnaPlantilla ENCF =
        new(nameof(FilaComprobante.ENCF), TipoDato.Texto, true, "e-NCF de 13 caracteres, por ejemplo E310000000001. El tipo de e-CF se deduce de él.");

    public static readonly ColumnaPlantilla FechaEmision =
        new(nameof(EncabezadoFila.FechaEmision), TipoDato.Fecha, true, "Fecha de emisión (dd-MM-yyyy).");

    public static readonly ColumnaPlantilla FechaVencimientoSecuencia =
        new(nameof(EncabezadoFila.FechaVencimientoSecuencia), TipoDato.Fecha, false, "Fecha de vencimiento de la secuencia. No aplica a los tipos 32 y 34.");

    public static readonly ColumnaPlantilla TipoPago =
        new(nameof(EncabezadoFila.TipoPago), TipoDato.Entero, false, "1 = Contado, 2 = Crédito, 3 = Gratuito.");

    public static readonly ColumnaPlantilla TipoIngreso =
        new(nameof(EncabezadoFila.TipoIngreso), TipoDato.Entero, false, "Tipo de ingreso DGII (1 a 6).");

    public static readonly ColumnaPlantilla FechaLimitePago =
        new(nameof(EncabezadoFila.FechaLimitePago), TipoDato.Fecha, false, "Fecha límite de pago (ventas a crédito).");

    public static readonly ColumnaPlantilla ArticulosIncluyenITBIS =
        new(nameof(EncabezadoFila.ArticulosIncluyenITBIS), TipoDato.Booleano, false, "Requerido en 31, 32, 33, 34, 41 y 45. Si = el precio ya incluye ITBIS; No = el ITBIS se suma al precio. Acepta Si/No o 1/0.");

    public static readonly ColumnaPlantilla RNCComprador =
        new(nameof(EncabezadoFila.RNCComprador), TipoDato.Texto, false, "RNC o cédula del comprador nacional.");

    public static readonly ColumnaPlantilla IdentificadorExtranjero =
        new(nameof(EncabezadoFila.IdentificadorExtranjero), TipoDato.Texto, false, "Identificador del comprador extranjero. No combinar con RNCComprador.");

    public static readonly ColumnaPlantilla RazonSocialComprador =
        new(nameof(EncabezadoFila.RazonSocialComprador), TipoDato.Texto, false, "Nombre o razón social del comprador.");

    public static readonly ColumnaPlantilla NCFModificado =
        new(nameof(EncabezadoFila.NCFModificado), TipoDato.Texto, false, "Solo notas 33 y 34: comprobante que se modifica.");

    public static readonly ColumnaPlantilla FechaNCFModificado =
        new(nameof(EncabezadoFila.FechaNCFModificado), TipoDato.Fecha, false, "Solo notas 33 y 34: fecha del comprobante modificado.");

    public static readonly ColumnaPlantilla CodigoModificacion =
        new(nameof(EncabezadoFila.CodigoModificacion), TipoDato.Entero, false, "Solo notas 33 y 34: código de modificación DGII (1 a 5).");

    public static readonly ColumnaPlantilla TipoMoneda =
        new(nameof(EncabezadoFila.TipoMoneda), TipoDato.Texto, false, "Moneda distinta al peso, por ejemplo USD. Requiere TipoCambio.");

    public static readonly ColumnaPlantilla TipoCambio =
        new(nameof(EncabezadoFila.TipoCambio), TipoDato.Decimal, false, "Tasa de cambio. Requiere TipoMoneda.");

    public static readonly ColumnaPlantilla NombreItem =
        new(nameof(DetalleFila.NombreItem), TipoDato.Texto, true, "Nombre del bien o servicio (máximo 80 caracteres).");

    public static readonly ColumnaPlantilla BienServicio =
        new(nameof(DetalleFila.BienServicio), TipoDato.Entero, true, "1 = Bien, 2 = Servicio.");

    public static readonly ColumnaPlantilla TipoITBIS =
        new(nameof(DetalleFila.TipoITBIS), TipoDato.Entero, true, "1 = 18 %, 2 = 16 %, 3 = 0 %, 4 = Exento.");

    public static readonly ColumnaPlantilla Cantidad =
        new(nameof(DetalleFila.Cantidad), TipoDato.Decimal, true, "Cantidad (hasta 2 decimales).");

    public static readonly ColumnaPlantilla PrecioUnitario =
        new(nameof(DetalleFila.PrecioUnitario), TipoDato.Decimal, true, "Precio unitario.");

    public static readonly ColumnaPlantilla IndicadorAgenteRetencion =
        new(nameof(DetalleFila.IndicadorAgenteRetencion), TipoDato.Entero, false, "Solo tipos 41 y 47: 1 = Agente de retención, 2 = Agente de percepción.");

    public static readonly ColumnaPlantilla MontoITBISRetenido =
        new(nameof(DetalleFila.MontoITBISRetenido), TipoDato.Decimal, false, "Solo tipos 41 y 47: ITBIS retenido en la línea.");

    public static readonly ColumnaPlantilla MontoISRRetenido =
        new(nameof(DetalleFila.MontoISRRetenido), TipoDato.Decimal, false, "Solo tipos 41 y 47: ISR retenido en la línea.");

    /// <summary>Todas las columnas, en el orden en que aparecen en la plantilla.</summary>
    public static readonly IReadOnlyList<ColumnaPlantilla> Todas =
    [
        Empresa, ENCF, FechaEmision, FechaVencimientoSecuencia, TipoPago, TipoIngreso, FechaLimitePago,
        ArticulosIncluyenITBIS, RNCComprador, IdentificadorExtranjero, RazonSocialComprador,
        NCFModificado, FechaNCFModificado, CodigoModificacion, TipoMoneda, TipoCambio,
        NombreItem, BienServicio, TipoITBIS, Cantidad, PrecioUnitario,
        IndicadorAgenteRetencion, MontoITBISRetenido, MontoISRRetenido
    ];
}
