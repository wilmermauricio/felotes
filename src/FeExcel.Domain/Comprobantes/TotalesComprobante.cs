namespace FeExcel.Domain.Comprobantes;

/// <summary>Totales del comprobante calculados por la librería DGII.</summary>
public sealed class TotalesComprobante
{
    public decimal? MontoGravadoTotal { get; init; }

    public decimal? MontoExento { get; init; }

    public decimal? TotalItbis { get; init; }

    public required decimal MontoTotal { get; init; }
}
