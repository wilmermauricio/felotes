using DGII.FE.XML.Models.Externo;
using FeExcel.Application.Abstracciones;

namespace FeExcel.Infrastructure.Emisores;

/// <summary>
/// Emisor genérico para poder validar sin base de datos: devuelve el mismo emisor para cualquier empresa.
/// </summary>
public sealed class EmisorProviderStub : IEmisorProvider
{
    public Task<Emisor?> ObtenerPorEmpresaAsync(string empresa, CancellationToken cancellationToken)
    {
        // TODO: reemplazar por la consulta a su base de datos. Ejemplo:
        //
        //   var empresaBd = await _db.Empresas.SingleOrDefaultAsync(e => e.Codigo == empresa, cancellationToken);
        //   if (empresaBd is null) return null;              // → "No se encontró el emisor para la empresa ..."
        //   return new Emisor
        //   {
        //       RNCEmisor = empresaBd.Rnc,                    // 9 u 11 dígitos
        //       RazonSocialEmisor = empresaBd.RazonSocial,
        //       DireccionEmisor = empresaBd.Direccion,
        //       Municipio = empresaBd.Municipio,
        //       Provincia = empresaBd.Provincia,
        //       ...
        //   };
        //
        // Luego registre su clase en DependencyInjection.AddInfrastructure() en lugar de esta.
        var emisor = new Emisor
        {
            RNCEmisor = "000000000",
            RazonSocialEmisor = $"EMISOR DE VALIDACION ({empresa})",
            DireccionEmisor = "N/D",
            Municipio = "010100",
            Provincia = "010000"
        };

        return Task.FromResult<Emisor?>(emisor);
    }
}
