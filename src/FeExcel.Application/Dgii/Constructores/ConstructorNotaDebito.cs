using DGII.FE.XML.Models.Externo;
using DGII.FE.XML.Models.Externo.Comprobantes;
using DGII.FE.XML.Models.Externo.Detalles;
using FeExcel.Application.Importacion;
using FeExcel.Domain.Comprobantes;

namespace FeExcel.Application.Dgii.Constructores;

internal sealed class ConstructorNotaDebito : IConstructorComprobante
{
    public TipoEcf Tipo => TipoEcf.NotaDebito;

    public Comprobante Construir(GrupoComprobante grupo, Emisor emisor)
    {
        var encabezado = grupo.Encabezado;

        var comprobante = new NotaDebitoElectronica
        {
            TipoPago = encabezado.TipoPago,
            TipoIngreso = encabezado.TipoIngreso,
            FechaVencimientoSecuencia = encabezado.FechaVencimientoSecuencia,
            FechaLimitePago = encabezado.FechaLimitePago,
            ArticulosIncluyenITBIS = encabezado.ArticulosIncluyenITBIS,
            Comprador = ConstruccionDgii.Comprador(encabezado)!,
            InformacionReferencia = ConstruccionDgii.Referencia(encabezado),
            Detalles = ConstruccionDgii.Detalles<DetalleNotaDebito>(grupo)
        };

        return ConstruccionDgii.ConDatosComunes(comprobante, grupo, emisor);
    }
}
