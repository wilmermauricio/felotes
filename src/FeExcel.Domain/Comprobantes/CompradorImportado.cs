namespace FeExcel.Domain.Comprobantes;

/// <summary>
/// Comprador del comprobante. Trae <see cref="RncComprador"/> (nacional)
/// o <see cref="IdentificadorExtranjero"/> (extranjero), nunca ambos.
/// </summary>
public sealed class CompradorImportado
{
    public string? RncComprador { get; init; }

    public string? IdentificadorExtranjero { get; init; }

    public string? RazonSocial { get; init; }

    public bool EsExtranjero => IdentificadorExtranjero is not null;
}
