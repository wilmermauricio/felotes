using DGII.FE.XML.Models.Externo;
using FeExcel.Application.Importacion;
using FeExcel.Domain.Comprobantes;
using FeExcel.Domain.Errores;

namespace FeExcel.Application.Dgii;

/// <summary>Valida un comprobante con las reglas de la librería DGII y obtiene los montos que ella calcula.</summary>
public interface IValidadorDgii
{
    ResultadoValidacionDgii Validar(TipoEcf tipo, GrupoComprobante grupo, Emisor emisor);
}

/// <summary>
/// Si <see cref="EsValido"/>, trae los totales y el monto de cada línea (en el mismo orden que las filas del grupo);
/// si no, los errores reportados por la librería.
/// </summary>
public sealed class ResultadoValidacionDgii
{
    public IReadOnlyList<ErrorImportacion> Errores { get; private init; } = [];

    public TotalesComprobante? Totales { get; private init; }

    public IReadOnlyList<decimal> MontosPorLinea { get; private init; } = [];

    public bool EsValido => Errores.Count == 0;

    public static ResultadoValidacionDgii Exitoso(TotalesComprobante totales, IReadOnlyList<decimal> montosPorLinea) =>
        new() { Totales = totales, MontosPorLinea = montosPorLinea };

    public static ResultadoValidacionDgii ConError(ErrorImportacion error) =>
        new() { Errores = [error] };
}
