using DGII.FE.XML.Models.Externo;

namespace FeExcel.Application.Abstracciones;

/// <summary>
/// Obtiene los datos del emisor de una empresa. El Excel no trae datos del emisor:
/// se resuelven aquí a partir de la columna "Empresa".
/// </summary>
public interface IEmisorProvider
{
    /// <returns>El emisor de la empresa, o <c>null</c> si la empresa no existe.</returns>
    Task<Emisor?> ObtenerPorEmpresaAsync(string empresa, CancellationToken cancellationToken);
}
