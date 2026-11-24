namespace FeExcel.Domain.Comprobantes;

/// <summary>Línea de detalle de un comprobante. Cada línea proviene de una fila del Excel.</summary>
public sealed class DetalleImportado
{
    /// <summary>Número de línea dentro del comprobante, desde 1.</summary>
    public required int NumeroLinea { get; init; }

    /// <summary>Fila del Excel de la que salió esta línea (útil para trazabilidad).</summary>
    public required int FilaExcel { get; init; }

    public required string NombreItem { get; init; }

    /// <summary>1 = Bien, 2 = Servicio.</summary>
    public required int BienServicio { get; init; }

    /// <summary>Indicador de facturación: 1 = 18 %, 2 = 16 %, 3 = 0 %, 4 = Exento.</summary>
    public required int TipoItbis { get; init; }

    public required decimal Cantidad { get; init; }

    public required decimal PrecioUnitario { get; init; }

    /// <summary>Monto de la línea calculado por la librería DGII.</summary>
    public required decimal MontoItem { get; init; }

    // Retención: solo tipos 41 (Compras) y 47 (Pagos al Exterior).

    /// <summary>1 = Agente de retención, 2 = Agente de percepción.</summary>
    public int? IndicadorAgenteRetencion { get; init; }

    public decimal? MontoItbisRetenido { get; init; }

    public decimal? MontoIsrRetenido { get; init; }
}
