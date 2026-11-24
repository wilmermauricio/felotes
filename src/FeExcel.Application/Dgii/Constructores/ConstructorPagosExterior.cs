using DGII.FE.XML.Models.Externo;
using DGII.FE.XML.Models.Externo.Comprobantes;
using DGII.FE.XML.Models.Externo.Detalles;
using FeExcel.Application.Importacion;
using FeExcel.Domain.Comprobantes;

namespace FeExcel.Application.Dgii.Constructores;

internal sealed class ConstructorPagosExterior : IConstructorComprobante
{
    public TipoEcf Tipo => TipoEcf.PagosExterior;

    public Comprobante Construir(GrupoComprobante grupo, Emisor emisor)
    {
        var encabezado = grupo.Encabezado;

        var comprobante = new ComprobantePagoExterior
        {
            TipoPago = encabezado.TipoPago,
            FechaVencimientoSecuencia = encabezado.FechaVencimientoSecuencia,
            FechaLimitePago = encabezado.FechaLimitePago,
            Comprador = ConstruccionDgii.Comprador(encabezado)!,
            Detalles = ConstruccionDgii.Detalles(grupo, fila => new DetalleComprobantePagoExterior
            {
                Retencion = ConstruccionDgii.Retencion(fila.Detalle)
            })
        };

        return ConstruccionDgii.ConDatosComunes(comprobante, grupo, emisor);
    }
}
