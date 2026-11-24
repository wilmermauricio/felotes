using FeExcel.Application.Importacion;
using FeExcel.Domain.Comprobantes;

namespace FeExcel.Web.Models;

public sealed record VistaPreviaViewModel(
    Guid Id,
    string NombreArchivo,
    bool PuedeConfirmar,
    ResumenCarga Resumen,
    IReadOnlyList<ResumenTipo> ResumenPorTipo,
    IReadOnlyList<string> ErroresGenerales,
    IReadOnlyList<ComprobanteVistaPrevia> Comprobantes)
{
    public static VistaPreviaViewModel Desde(ResultadoImportacion resultado)
    {
        var comprobantes = resultado.Comprobantes.Select(ComprobanteVistaPrevia.Desde).ToList();

        return new VistaPreviaViewModel(
            resultado.Id,
            resultado.NombreArchivo,
            PuedeConfirmar: !resultado.TieneErrores && resultado.CantidadValidos > 0,
            new ResumenCarga(
                TotalComprobantes: comprobantes.Count,
                TotalLineas: comprobantes.Sum(c => c.Detalles.Count),
                Validos: resultado.CantidadValidos,
                ConErrores: resultado.CantidadConErrores),
            ResumenTipo.Agrupar(resultado.Comprobantes),
            resultado.ErroresGenerales.Select(e => e.ToString()).ToList(),
            comprobantes);
    }
}

public sealed record ResumenCarga(int TotalComprobantes, int TotalLineas, int Validos, int ConErrores);

/// <summary>Una fila del resumen por tipo de e-CF. El monto suma solo los comprobantes válidos.</summary>
public sealed record ResumenTipo(string Tipo, int Cantidad, int ConErrores, decimal MontoTotal)
{
    private const string TipoDesconocido = "Desconocido";

    public static IReadOnlyList<ResumenTipo> Agrupar(IEnumerable<ComprobanteProcesado> comprobantes) =>
        comprobantes
            .GroupBy(c => c.Tipo)
            .OrderBy(g => g.Key is null)
            .ThenBy(g => g.Key)
            .Select(g => new ResumenTipo(
                Etiqueta(g.Key),
                g.Count(),
                g.Count(c => !c.EsValido),
                g.Sum(c => c.Comprobante?.Totales.MontoTotal ?? 0)))
            .ToList();

    public static string Etiqueta(TipoEcf? tipo) =>
        tipo is { } t ? $"{t.Codigo()} - {t.Descripcion()}" : TipoDesconocido;
}

/// <summary>
/// Un comprobante en la vista previa. Si es válido, los datos salen del comprobante ya validado
/// (con montos calculados); si tiene errores, de las filas del Excel tal como vinieron.
/// </summary>
public sealed record ComprobanteVistaPrevia(
    string Empresa,
    string Encf,
    string Tipo,
    DateTime? FechaEmision,
    string? Comprador,
    decimal? TotalItbis,
    decimal? MontoTotal,
    bool EsValido,
    IReadOnlyList<string> Errores,
    IReadOnlyList<DetalleVistaPrevia> Detalles,
    string TextoBusqueda)
{
    public static ComprobanteVistaPrevia Desde(ComprobanteProcesado procesado)
    {
        var grupo = procesado.Grupo;
        var encabezado = grupo.Encabezado;
        var comprobante = procesado.Comprobante;
        var tipo = ResumenTipo.Etiqueta(procesado.Tipo);
        var errores = procesado.Errores.Select(e => e.ToString()).ToList();
        var detalles = comprobante is not null ? DetallesValidados(comprobante) : DetallesDeFilas(grupo);
        var totalItbis = comprobante?.Totales.TotalItbis;
        var montoTotal = comprobante?.Totales.MontoTotal;

        return new ComprobanteVistaPrevia(
            grupo.Empresa,
            grupo.Encf,
            tipo,
            encabezado.FechaEmision,
            encabezado.RazonSocialComprador ?? encabezado.RNCComprador ?? encabezado.IdentificadorExtranjero,
            totalItbis,
            montoTotal,
            procesado.EsValido,
            errores,
            detalles,
            Busqueda.Texto(grupo.Empresa, grupo.Encf, tipo, encabezado, totalItbis, montoTotal, detalles, errores));
    }

    private static List<DetalleVistaPrevia> DetallesValidados(ComprobanteImportado comprobante) =>
        comprobante.Detalles
            .Select(d => new DetalleVistaPrevia(d.NumeroLinea, d.FilaExcel, d.NombreItem, d.Cantidad, d.PrecioUnitario, d.MontoItem))
            .ToList();

    private static List<DetalleVistaPrevia> DetallesDeFilas(GrupoComprobante grupo) =>
        grupo.Filas
            .Select((f, i) => new DetalleVistaPrevia(i + 1, f.NumeroFila, f.Detalle.NombreItem, f.Detalle.Cantidad, f.Detalle.PrecioUnitario, null))
            .ToList();
}

public sealed record DetalleVistaPrevia(
    int Linea,
    int FilaExcel,
    string? Nombre,
    decimal? Cantidad,
    decimal? PrecioUnitario,
    decimal? MontoItem);
