using FeExcel.Application.Abstracciones;
using FeExcel.Application.Importacion;
using Microsoft.Extensions.Caching.Memory;

namespace FeExcel.Infrastructure.Cache;

/// <summary>
/// Guarda las importaciones en la memoria del servidor durante 30 minutos.
/// Solo sirve para un servidor; con varias instancias habría que usar una caché distribuida.
/// </summary>
public sealed class AlmacenImportacionesMemoria(IMemoryCache cache) : IAlmacenImportaciones
{
    private static readonly TimeSpan Duracion = TimeSpan.FromMinutes(30);
    private readonly Lock _bloqueo = new();

    public void Guardar(ResultadoImportacion resultado) =>
        cache.Set(Clave(resultado.Id), resultado, Duracion);

    public ResultadoImportacion? Obtener(Guid id) =>
        cache.TryGetValue(Clave(id), out ResultadoImportacion? resultado) ? resultado : null;

    public ResultadoImportacion? Retirar(Guid id)
    {
        lock (_bloqueo)
        {
            var resultado = Obtener(id);
            if (resultado is not null)
                cache.Remove(Clave(id));

            return resultado;
        }
    }

    private static string Clave(Guid id) => $"importacion:{id}";
}
