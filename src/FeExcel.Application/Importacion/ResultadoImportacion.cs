using FeExcel.Domain.Comprobantes;
using FeExcel.Domain.Errores;

namespace FeExcel.Application.Importacion;

/// <summary>Un comprobante del archivo: válido (con su <see cref="Comprobante"/>) o con errores.</summary>
public sealed class ComprobanteProcesado
{
    public required GrupoComprobante Grupo { get; init; }

    /// <summary><c>null</c> si el ENCF no permitió deducir el tipo.</summary>
    public TipoEcf? Tipo { get; init; }

    public IReadOnlyList<ErrorImportacion> Errores { get; init; } = [];

    /// <summary>Solo existe cuando el comprobante pasó todas las validaciones.</summary>
    public ComprobanteImportado? Comprobante { get; init; }

    public bool EsValido => Comprobante is not null;

    public static ComprobanteProcesado Valido(GrupoComprobante grupo, ComprobanteImportado comprobante) =>
        new() { Grupo = grupo, Tipo = comprobante.TipoEcf, Comprobante = comprobante };

    public static ComprobanteProcesado ConErrores(GrupoComprobante grupo, TipoEcf? tipo, IReadOnlyList<ErrorImportacion> errores) =>
        new() { Grupo = grupo, Tipo = tipo, Errores = errores };
}

/// <summary>Resultado completo de procesar un archivo. Es lo que se muestra en la vista previa.</summary>
public sealed class ResultadoImportacion
{
    public Guid Id { get; } = Guid.NewGuid();

    public required string NombreArchivo { get; init; }

    /// <summary>Errores que no pertenecen a un comprobante: archivo inválido o filas sin Empresa/ENCF.</summary>
    public IReadOnlyList<ErrorImportacion> ErroresGenerales { get; init; } = [];

    public IReadOnlyList<ComprobanteProcesado> Comprobantes { get; init; } = [];

    public int CantidadValidos => Comprobantes.Count(c => c.EsValido);

    public int CantidadConErrores => Comprobantes.Count(c => !c.EsValido);

    public bool TieneErrores => ErroresGenerales.Count > 0 || CantidadConErrores > 0;

    public IReadOnlyList<ComprobanteImportado> ComprobantesValidos =>
        Comprobantes.Where(c => c.EsValido).Select(c => c.Comprobante!).ToList();

    public static ResultadoImportacion Rechazado(string nombreArchivo, IReadOnlyList<ErrorImportacion> errores) =>
        new() { NombreArchivo = nombreArchivo, ErroresGenerales = errores };
}
