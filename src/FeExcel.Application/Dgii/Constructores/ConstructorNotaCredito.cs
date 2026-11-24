using DGII.FE.XML.Models.Externo;
using DGII.FE.XML.Models.Externo.Comprobantes;
using DGII.FE.XML.Models.Externo.Detalles;
using FeExcel.Application.Importacion;
using FeExcel.Domain.Comprobantes;

namespace FeExcel.Application.Dgii.Constructores;

internal sealed class ConstructorNotaCredito : IConstructorComprobante
{
    /// <summary>
    /// La DGII marca con IndicadorNotaCredito = 1 las notas de crédito emitidas más de 30 días
    /// después del comprobante que modifican. Se deduce de las fechas para no pedir otra columna.
    /// </summary>
    private const int DiasLimiteNotaCredito = 30;

    public TipoEcf Tipo => TipoEcf.NotaCredito;

    public Comprobante Construir(GrupoComprobante grupo, Emisor emisor)
    {
        var encabezado = grupo.Encabezado;
        var diasDesdeComprobanteModificado =
            (encabezado.FechaEmision!.Value - encabezado.FechaNCFModificado!.Value).TotalDays;

        var comprobante = new NotaCreditoElectronica
        {
            TipoPago = encabezado.TipoPago,
            TipoIngreso = encabezado.TipoIngreso,
            FechaLimitePago = encabezado.FechaLimitePago,
            ArticulosIncluyenITBIS = encabezado.ArticulosIncluyenITBIS,
            Comprador = ConstruccionDgii.Comprador(encabezado)!,
            InformacionReferencia = ConstruccionDgii.Referencia(encabezado),
            EsNotaCredito = diasDesdeComprobanteModificado > DiasLimiteNotaCredito,
            Detalles = ConstruccionDgii.Detalles<DetalleNotaCedito>(grupo)
        };

        return ConstruccionDgii.ConDatosComunes(comprobante, grupo, emisor);
    }
}
