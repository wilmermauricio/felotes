using DGII.FE.XML.Models.Externo;
using DGII.FE.XML.Models.Externo.Comprobantes;
using DGII.FE.XML.Models.Externo.Detalles;
using FeExcel.Application.Importacion;
using FeExcel.Domain.Comprobantes;

namespace FeExcel.Application.Dgii.Constructores;

/// <summary>Gastos Menores no lleva comprador.</summary>
internal sealed class ConstructorGastosMenores : IConstructorComprobante
{
    public TipoEcf Tipo => TipoEcf.GastosMenores;

    public Comprobante Construir(GrupoComprobante grupo, Emisor emisor)
    {
        var encabezado = grupo.Encabezado;

        var comprobante = new ComprobanteGastosMenores
        {
            TipoPago = encabezado.TipoPago,
            FechaVencimientoSecuencia = encabezado.FechaVencimientoSecuencia,
            Detalles = ConstruccionDgii.Detalles<DetalleComprobanteGastosMenores>(grupo)
        };

        return ConstruccionDgii.ConDatosComunes(comprobante, grupo, emisor);
    }
}
