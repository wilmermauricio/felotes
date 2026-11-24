using DGII.FE.XML.Models.Externo;
using DGII.FE.XML.Models.Externo.Comprobantes;
using DGII.FE.XML.Models.Externo.Detalles;
using FeExcel.Application.Importacion;
using FeExcel.Domain.Comprobantes;

namespace FeExcel.Application.Dgii.Constructores;

internal sealed class ConstructorGubernamental : IConstructorComprobante
{
    public TipoEcf Tipo => TipoEcf.Gubernamental;

    public Comprobante Construir(GrupoComprobante grupo, Emisor emisor)
    {
        var encabezado = grupo.Encabezado;

        var comprobante = new ComprobanteGubernamental
        {
            TipoPago = encabezado.TipoPago,
            FechaVencimientoSecuencia = encabezado.FechaVencimientoSecuencia,
            FechaLimitePago = encabezado.FechaLimitePago,
            ArticulosIncluyenITBIS = encabezado.ArticulosIncluyenITBIS,
            Comprador = ConstruccionDgii.Comprador(encabezado)!,
            Detalles = ConstruccionDgii.Detalles<DetalleComprobanteGubernamental>(grupo)
        };

        // TipoIngreso es opcional en este tipo, pero su setter rechaza null: solo se asigna si viene.
        if (encabezado.TipoIngreso is not null)
            comprobante.TipoIngreso = encabezado.TipoIngreso;

        return ConstruccionDgii.ConDatosComunes(comprobante, grupo, emisor);
    }
}
