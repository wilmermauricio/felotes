using DGII.FE.XML.Models.Externo;
using DGII.FE.XML.Models.Externo.Comprobantes;
using DGII.FE.XML.Models.Externo.Detalles;
using FeExcel.Application.Importacion;
using FeExcel.Domain.Comprobantes;

namespace FeExcel.Application.Dgii.Constructores;

internal sealed class ConstructorFacturaCreditoFiscal : IConstructorComprobante
{
    public TipoEcf Tipo => TipoEcf.FacturaCreditoFiscal;

    public Comprobante Construir(GrupoComprobante grupo, Emisor emisor)
    {
        var encabezado = grupo.Encabezado;

        var comprobante = new FacturaCreditoFiscalElectronica
        {
            TipoPago = encabezado.TipoPago,
            TipoIngreso = encabezado.TipoIngreso,
            FechaVencimientoSecuencia = encabezado.FechaVencimientoSecuencia!.Value,
            FechaLimitePago = encabezado.FechaLimitePago,
            ArticulosIncluyenITBIS = encabezado.ArticulosIncluyenITBIS,
            Comprador = ConstruccionDgii.Comprador(encabezado)!,
            Detalles = ConstruccionDgii.Detalles<DetalleFacturaCreditoFiscalElectronica>(grupo)
        };

        return ConstruccionDgii.ConDatosComunes(comprobante, grupo, emisor);
    }
}
