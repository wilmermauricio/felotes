namespace FeExcel.Domain.Comprobantes;

/// <summary>Comprobante que modifica una nota de débito o de crédito.</summary>
public sealed class ReferenciaImportada
{
    public required string NcfModificado { get; init; }

    public required DateTime FechaNcfModificado { get; init; }

    /// <summary>Código de modificación de la DGII (1 a 5).</summary>
    public required int CodigoModificacion { get; init; }
}
