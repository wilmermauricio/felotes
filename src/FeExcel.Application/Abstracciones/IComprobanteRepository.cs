using FeExcel.Domain.Comprobantes;

namespace FeExcel.Application.Abstracciones;

/// <summary>
/// Persiste los comprobantes confirmados. Aquí se implementa el mapeo
/// de <see cref="ComprobanteImportado"/> y <see cref="DetalleImportado"/> a la base de datos.
/// </summary>
public interface IComprobanteRepository
{
    /// <summary>Recibe en una sola llamada todos los comprobantes de una importación confirmada.</summary>
    Task GuardarAsync(IReadOnlyList<ComprobanteImportado> comprobantes, CancellationToken cancellationToken);
}
