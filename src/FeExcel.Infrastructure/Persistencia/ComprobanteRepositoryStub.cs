using FeExcel.Application.Abstracciones;
using FeExcel.Domain.Comprobantes;
using Microsoft.Extensions.Logging;

namespace FeExcel.Infrastructure.Persistencia;

/// <summary>
/// Repositorio de prueba: no persiste nada, solo registra en el log lo que recibió.
/// </summary>
public sealed class ComprobanteRepositoryStub(ILogger<ComprobanteRepositoryStub> logger) : IComprobanteRepository
{
    public Task GuardarAsync(IReadOnlyList<ComprobanteImportado> comprobantes, CancellationToken cancellationToken)
    {
        // TODO: reemplazar por el mapeo a su base de datos. Por cada comprobante:
        //
        //   ComprobanteImportado  → su tabla de encabezado
        //     .Empresa, .Encf, .TipoEcf (int: 31..47), .FechaEmision, .TipoPago, ...
        //     .Comprador?           → columnas del comprador (RncComprador o IdentificadorExtranjero)
        //     .Referencia?          → solo notas 33/34 (NcfModificado, FechaNcfModificado, CodigoModificacion)
        //     .Totales              → MontoGravadoTotal, MontoExento, TotalItbis, MontoTotal (calculados por la librería DGII)
        //
        //   DetalleImportado      → su tabla de detalle
        //     .NumeroLinea, .NombreItem, .BienServicio, .TipoItbis, .Cantidad, .PrecioUnitario, .MontoItem
        //     .FilaExcel sirve para rastrear la fila de origen si algo falla.
        //
        // Guarde todo en una sola transacción: la importación es todo o nada.
        // Luego registre su clase en DependencyInjection.AddInfrastructure() en lugar de esta.
        foreach (var porEmpresa in comprobantes.GroupBy(c => c.Empresa))
        {
            logger.LogInformation(
                "Comprobantes recibidos para la empresa {Empresa}: {Cantidad} ({Detalles} líneas de detalle)",
                porEmpresa.Key, porEmpresa.Count(), porEmpresa.Sum(c => c.Detalles.Count));
        }

        logger.LogInformation("Total de comprobantes recibidos: {Cantidad}", comprobantes.Count);
        return Task.CompletedTask;
    }
}
