using FeExcel.Application.Abstracciones;
using FeExcel.Infrastructure.Cache;
using FeExcel.Infrastructure.Emisores;
using FeExcel.Infrastructure.Excel;
using FeExcel.Infrastructure.Persistencia;
using Microsoft.Extensions.DependencyInjection;

namespace FeExcel.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<IAlmacenImportaciones, AlmacenImportacionesMemoria>();

        services.AddSingleton<ILectorPlantilla, LectorPlantillaClosedXml>();
        services.AddSingleton<IGeneradorPlantilla, GeneradorPlantillaClosedXml>();

        // Puntos de conexión con su base de datos: cambie estos stubs por sus implementaciones.
        services.AddScoped<IEmisorProvider, EmisorProviderStub>();
        services.AddScoped<IComprobanteRepository, ComprobanteRepositoryStub>();

        return services;
    }
}
