using FeExcel.Application.Plantilla;
using FeExcel.Domain.Errores;

namespace FeExcel.Application.Importacion;

public sealed record ResultadoAgrupacion(IReadOnlyList<GrupoComprobante> Grupos, IReadOnlyList<ErrorImportacion> Errores);

/// <summary>
/// Agrupa las filas por (Empresa, ENCF). La comparación ignora mayúsculas y espacios,
/// y los grupos conservan el orden en que aparece su primera fila.
/// </summary>
public sealed class AgrupadorComprobantes
{
    public ResultadoAgrupacion Agrupar(IEnumerable<FilaComprobante> filas)
    {
        var errores = new List<ErrorImportacion>();
        var filasConClave = new List<(string Empresa, string Encf, FilaComprobante Fila)>();

        foreach (var fila in filas)
        {
            var empresa = fila.Empresa?.Trim();
            var encf = fila.ENCF?.Trim().ToUpperInvariant();

            if (string.IsNullOrEmpty(empresa))
                errores.Add(ErrorImportacion.EnCelda(fila.NumeroFila, ColumnasPlantilla.Empresa.Nombre, "El valor es requerido"));

            if (string.IsNullOrEmpty(encf))
                errores.Add(ErrorImportacion.EnCelda(fila.NumeroFila, ColumnasPlantilla.ENCF.Nombre, "El valor es requerido"));

            if (!string.IsNullOrEmpty(empresa) && !string.IsNullOrEmpty(encf))
                filasConClave.Add((empresa, encf, fila));
        }

        var grupos = filasConClave
            .GroupBy(f => (Empresa: f.Empresa.ToUpperInvariant(), f.Encf))
            .Select(g => new GrupoComprobante(
                Empresa: g.First().Empresa,
                Encf: g.Key.Encf,
                Filas: g.Select(f => f.Fila).ToList()))
            .ToList();

        return new ResultadoAgrupacion(grupos, errores);
    }
}
