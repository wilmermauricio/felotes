using FeExcel.Application.Plantilla;

namespace FeExcel.Application.Importacion;

/// <summary>Filas del Excel que comparten Empresa y ENCF: un comprobante con una línea de detalle por fila.</summary>
public sealed record GrupoComprobante(string Empresa, string Encf, IReadOnlyList<FilaComprobante> Filas)
{
    /// <summary>Encabezado del comprobante (el de la primera fila; la consistencia entre filas se valida aparte).</summary>
    public EncabezadoFila Encabezado => Filas[0].Encabezado;

    public IEnumerable<int> NumerosFila => Filas.Select(f => f.NumeroFila);
}
