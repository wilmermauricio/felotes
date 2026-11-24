using DGII.FE.XML.Models.Externo;
using FeExcel.Application.Abstracciones;
using FeExcel.Application.Importacion;
using FeExcel.Application.Plantilla;
using FeExcel.Domain.Comprobantes;

namespace FeExcel.Application.Tests.Importacion;

internal sealed class LectorFijo(LecturaPlantilla lectura) : ILectorPlantilla
{
    public LecturaPlantilla Leer(Stream archivo) => lectura;
}

internal sealed class EmisoresFijos(params string[] empresasExistentes) : IEmisorProvider
{
    public Task<Emisor?> ObtenerPorEmpresaAsync(string empresa, CancellationToken cancellationToken) =>
        Task.FromResult(empresasExistentes.Contains(empresa)
            ? new Emisor { RNCEmisor = "131234567", RazonSocialEmisor = "Emisor SRL", Municipio = "010100", Provincia = "010000" }
            : null);
}

internal sealed class AlmacenEnMemoria : IAlmacenImportaciones
{
    private readonly Dictionary<Guid, ResultadoImportacion> _importaciones = [];

    public void Guardar(ResultadoImportacion resultado) => _importaciones[resultado.Id] = resultado;

    public ResultadoImportacion? Obtener(Guid id) => _importaciones.GetValueOrDefault(id);

    public ResultadoImportacion? Retirar(Guid id) => _importaciones.Remove(id, out var resultado) ? resultado : null;
}

internal sealed class RepositorioEspia : IComprobanteRepository
{
    public List<IReadOnlyList<ComprobanteImportado>> Llamadas { get; } = [];

    public Task GuardarAsync(IReadOnlyList<ComprobanteImportado> comprobantes, CancellationToken cancellationToken)
    {
        Llamadas.Add(comprobantes);
        return Task.CompletedTask;
    }
}
