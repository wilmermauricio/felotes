using FeExcel.Application.Dgii;
using FeExcel.Application.Dgii.Constructores;
using FeExcel.Application.Importacion;
using Microsoft.Extensions.DependencyInjection;

namespace FeExcel.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Reglas y validadores: no guardan estado, se comparten.
        services.AddSingleton<AgrupadorComprobantes>();
        services.AddSingleton<ValidadorConsistencia>();
        services.AddSingleton<ReglasPorTipoEcf>();
        services.AddSingleton<ProcesadorComprobante>();
        services.AddSingleton<IValidadorDgii, ValidadorComprobantesDgii>();

        // Un constructor por tipo de e-CF.
        services.AddSingleton<IConstructorComprobante, ConstructorFacturaCreditoFiscal>();
        services.AddSingleton<IConstructorComprobante, ConstructorFacturaConsumo>();
        services.AddSingleton<IConstructorComprobante, ConstructorNotaDebito>();
        services.AddSingleton<IConstructorComprobante, ConstructorNotaCredito>();
        services.AddSingleton<IConstructorComprobante, ConstructorCompras>();
        services.AddSingleton<IConstructorComprobante, ConstructorGastosMenores>();
        services.AddSingleton<IConstructorComprobante, ConstructorRegimenesEspeciales>();
        services.AddSingleton<IConstructorComprobante, ConstructorGubernamental>();
        services.AddSingleton<IConstructorComprobante, ConstructorExportaciones>();
        services.AddSingleton<IConstructorComprobante, ConstructorPagosExterior>();

        // Casos de uso: dependen de puertos que pueden usar la base de datos.
        services.AddScoped<ProcesarArchivoHandler>();
        services.AddScoped<ConfirmarImportacionHandler>();

        return services;
    }
}
