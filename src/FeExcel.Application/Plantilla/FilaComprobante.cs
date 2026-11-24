namespace FeExcel.Application.Plantilla;

/// <summary>
/// Una fila del Excel ya convertida a tipos .NET. Los valores vacíos o inválidos quedan en <c>null</c>
/// (los errores de conversión se reportan aparte, en <see cref="LecturaPlantilla"/>).
/// </summary>
public sealed record FilaComprobante(
    int NumeroFila,
    string? Empresa,
    string? ENCF,
    EncabezadoFila Encabezado,
    DetalleFila Detalle);

/// <summary>
/// Campos de encabezado del comprobante. Se repiten en cada fila del mismo comprobante
/// y deben ser iguales en todas ellas. Los nombres de las propiedades coinciden con las columnas del Excel.
/// </summary>
public sealed record EncabezadoFila
{
    public DateTime? FechaEmision { get; init; }
    public DateTime? FechaVencimientoSecuencia { get; init; }
    public int? TipoPago { get; init; }
    public int? TipoIngreso { get; init; }
    public DateTime? FechaLimitePago { get; init; }
    public bool? ArticulosIncluyenITBIS { get; init; }
    public string? RNCComprador { get; init; }
    public string? IdentificadorExtranjero { get; init; }
    public string? RazonSocialComprador { get; init; }
    public string? NCFModificado { get; init; }
    public DateTime? FechaNCFModificado { get; init; }
    public int? CodigoModificacion { get; init; }
    public string? TipoMoneda { get; init; }
    public decimal? TipoCambio { get; init; }
}

/// <summary>Campos de la línea de detalle. Los nombres coinciden con las columnas del Excel.</summary>
public sealed record DetalleFila
{
    public string? NombreItem { get; init; }
    public int? BienServicio { get; init; }
    public int? TipoITBIS { get; init; }
    public decimal? Cantidad { get; init; }
    public decimal? PrecioUnitario { get; init; }

    // Retención: solo aplica a Compras (41) y Pagos al Exterior (47).
    public int? IndicadorAgenteRetencion { get; init; }
    public decimal? MontoITBISRetenido { get; init; }
    public decimal? MontoISRRetenido { get; init; }
}
