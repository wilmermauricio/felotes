using DGII.FE.XML.Models.Externo;
using FeExcel.Application.Abstracciones;
using FeExcel.Domain.Errores;

namespace FeExcel.Application.Importacion;

/// <summary>Archivo recibido desde la interfaz web, sin depender de ASP.NET.</summary>
public sealed record ArchivoSubido(string Nombre, long Tamano, Stream Contenido);

/// <summary>
/// Caso de uso "subir archivo": lee el Excel, agrupa las filas en comprobantes, valida cada uno
/// y guarda el resultado para la vista previa.
/// </summary>
public sealed class ProcesarArchivoHandler(
    ILectorPlantilla lector,
    AgrupadorComprobantes agrupador,
    ProcesadorComprobante procesador,
    IEmisorProvider emisorProvider,
    IAlmacenImportaciones almacen)
{
    public const long TamanoMaximoBytes = 10 * 1024 * 1024;
    private const string ExtensionPermitida = ".xlsx";

    public async Task<ResultadoImportacion> EjecutarAsync(ArchivoSubido archivo, CancellationToken cancellationToken)
    {
        var resultado = await ProcesarAsync(archivo, cancellationToken);
        almacen.Guardar(resultado);
        return resultado;
    }

    private async Task<ResultadoImportacion> ProcesarAsync(ArchivoSubido archivo, CancellationToken cancellationToken)
    {
        var errorArchivo = ValidarArchivo(archivo);
        if (errorArchivo is not null)
            return ResultadoImportacion.Rechazado(archivo.Nombre, [errorArchivo]);

        var lectura = lector.Leer(archivo.Contenido);
        if (lectura.ErroresArchivo.Count > 0)
            return ResultadoImportacion.Rechazado(archivo.Nombre, lectura.ErroresArchivo);

        if (lectura.Filas.Count == 0)
            return ResultadoImportacion.Rechazado(archivo.Nombre, [ErrorImportacion.General("El archivo no contiene filas de datos")]);

        var agrupacion = agrupador.Agrupar(lectura.Filas);
        var erroresPorFila = lectura.ErroresCeldas.ToLookup(e => e.Fila);
        var emisores = await ObtenerEmisoresAsync(agrupacion.Grupos, cancellationToken);

        var comprobantes = agrupacion.Grupos
            .Select(grupo => procesador.Procesar(
                grupo,
                grupo.NumerosFila.SelectMany(fila => erroresPorFila[fila]).ToList(),
                emisores[grupo.Empresa]))
            .ToList();

        // Las filas sin Empresa o ENCF no pertenecen a ningún comprobante: sus errores van como generales.
        var filasAgrupadas = agrupacion.Grupos.SelectMany(g => g.NumerosFila).ToHashSet();
        var erroresSinComprobante = lectura.ErroresCeldas.Where(e => e.Fila is not { } fila || !filasAgrupadas.Contains(fila));

        return new ResultadoImportacion
        {
            NombreArchivo = archivo.Nombre,
            ErroresGenerales = [.. agrupacion.Errores, .. erroresSinComprobante],
            Comprobantes = comprobantes
        };
    }

    private static ErrorImportacion? ValidarArchivo(ArchivoSubido archivo)
    {
        if (!string.Equals(Path.GetExtension(archivo.Nombre), ExtensionPermitida, StringComparison.OrdinalIgnoreCase))
            return ErrorImportacion.General("Solo se aceptan archivos .xlsx");

        if (archivo.Tamano == 0)
            return ErrorImportacion.General("El archivo está vacío");

        if (archivo.Tamano > TamanoMaximoBytes)
            return ErrorImportacion.General("El archivo supera el tamaño máximo de 10 MB");

        return null;
    }

    /// <summary>Busca una sola vez el emisor de cada empresa del archivo.</summary>
    private async Task<Dictionary<string, Emisor?>> ObtenerEmisoresAsync(
        IEnumerable<GrupoComprobante> grupos, CancellationToken cancellationToken)
    {
        var emisores = new Dictionary<string, Emisor?>(StringComparer.OrdinalIgnoreCase);

        foreach (var empresa in grupos.Select(g => g.Empresa).Distinct(StringComparer.OrdinalIgnoreCase))
            emisores[empresa] = await emisorProvider.ObtenerPorEmpresaAsync(empresa, cancellationToken);

        return emisores;
    }
}
