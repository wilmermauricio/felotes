namespace FeExcel.Domain.Comprobantes;

/// <summary>
/// Comprobante importado desde Excel, ya agrupado y validado con las reglas de la DGII.
/// Es el objeto que recibe <c>IComprobanteRepository</c> para mapearlo a la base de datos.
/// </summary>
public sealed class ComprobanteImportado
{
    /// <summary>Código de la empresa tal como viene en el Excel. Con él se busca el emisor.</summary>
    public required string Empresa { get; init; }

    /// <summary>e-NCF de 13 caracteres, por ejemplo "E310000000001".</summary>
    public required string Encf { get; init; }

    public required TipoEcf TipoEcf { get; init; }

    public required DateTime FechaEmision { get; init; }

    public DateTime? FechaVencimientoSecuencia { get; init; }

    /// <summary>1 = Contado, 2 = Crédito, 3 = Gratuito.</summary>
    public int? TipoPago { get; init; }

    /// <summary>Código de tipo de ingreso de la DGII (01 a 06).</summary>
    public int? TipoIngreso { get; init; }

    public DateTime? FechaLimitePago { get; init; }

    /// <summary><c>true</c> si los precios de los detalles ya incluyen el ITBIS.</summary>
    public bool? ArticulosIncluyenItbis { get; init; }

    /// <summary>Código de moneda (por ejemplo "USD"). <c>null</c> cuando es en pesos.</summary>
    public string? TipoMoneda { get; init; }

    public decimal? TipoCambio { get; init; }

    public CompradorImportado? Comprador { get; init; }

    /// <summary>Solo en notas de débito (33) y crédito (34).</summary>
    public ReferenciaImportada? Referencia { get; init; }

    public required IReadOnlyList<DetalleImportado> Detalles { get; init; }

    /// <summary>Totales calculados por la librería DGII.</summary>
    public required TotalesComprobante Totales { get; init; }
}
