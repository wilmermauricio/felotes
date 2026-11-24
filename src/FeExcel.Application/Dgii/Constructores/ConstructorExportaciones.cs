using DGII.FE.XML.Models.Externo;
using DGII.FE.XML.Models.Externo.Comprobantes;
using DGII.FE.XML.Models.Externo.Detalles;
using FeExcel.Application.Importacion;
using FeExcel.Domain.Comprobantes;

namespace FeExcel.Application.Dgii.Constructores;

internal sealed class ConstructorExportaciones : IConstructorComprobante
{
    public TipoEcf Tipo => TipoEcf.Exportaciones;

    public Comprobante Construir(GrupoComprobante grupo, Emisor emisor)
    {
        var encabezado = grupo.Encabezado;

        var comprobante = new ComprobanteExportacion
        {
            TipoPago = encabezado.TipoPago,
            TipoIngreso = encabezado.TipoIngreso,
            FechaVencimientoSecuencia = encabezado.FechaVencimientoSecuencia,
            FechaLimitePago = encabezado.FechaLimitePago,
            Comprador = ConstruccionDgii.Comprador(encabezado)!,
            Detalles = ConstruccionDgii.Detalles<DetalleComprobanteExportacion>(grupo)
        };

        return ConstruccionDgii.ConDatosComunes(comprobante, grupo, emisor);
    }
}
