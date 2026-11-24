using DGII.FE.XML.Models.Externo;
using FeExcel.Application.Dgii;
using FeExcel.Domain.Comprobantes;
using FeExcel.Domain.Errores;

namespace FeExcel.Application.Importacion;

/// <summary>
/// Valida un comprobante (grupo de filas) por etapas. Cada etapa se ejecuta solo si la anterior
/// no tuvo errores, para no pasar datos incompletos a la librería DGII.
/// </summary>
public sealed class ProcesadorComprobante(
    ValidadorConsistencia validadorConsistencia,
    ReglasPorTipoEcf reglasPorTipo,
    IValidadorDgii validadorDgii)
{
    /// <param name="erroresCeldas">Errores de formato de las celdas de este grupo.</param>
    /// <param name="emisor">Emisor de la empresa del grupo; <c>null</c> si no se encontró.</param>
    public ComprobanteProcesado Procesar(GrupoComprobante grupo, IReadOnlyList<ErrorImportacion> erroresCeldas, Emisor? emisor)
    {
        var tipoValido = TipoEcfExtensions.TryDesdeEncf(grupo.Encf, out var tipo, out var errorTipo);
        TipoEcf? tipoDeducido = tipoValido ? tipo : null;

        List<ErrorImportacion> errores = [.. erroresCeldas];
        if (!tipoValido)
            errores.Add(new ErrorImportacion(null, "ENCF", errorTipo));
        if (errores.Count > 0)
            return ComprobanteProcesado.ConErrores(grupo, tipoDeducido, errores);

        var erroresConsistencia = validadorConsistencia.Validar(grupo);
        if (erroresConsistencia.Count > 0)
            return ComprobanteProcesado.ConErrores(grupo, tipo, erroresConsistencia);

        var erroresReglas = reglasPorTipo.Validar(tipo, grupo);
        if (erroresReglas.Count > 0)
            return ComprobanteProcesado.ConErrores(grupo, tipo, erroresReglas);

        if (emisor is null)
            return ComprobanteProcesado.ConErrores(grupo, tipo,
                [ErrorImportacion.General($"No se encontró el emisor para la empresa '{grupo.Empresa}'")]);

        var validacion = validadorDgii.Validar(tipo, grupo, emisor);
        if (!validacion.EsValido)
            return ComprobanteProcesado.ConErrores(grupo, tipo, validacion.Errores);

        return ComprobanteProcesado.Valido(grupo, MapeadorComprobanteImportado.Mapear(tipo, grupo, validacion));
    }
}
