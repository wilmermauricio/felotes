using DGII.FE.XML.Models.Externo.DetalleImpuestosAdicionales;
using DGII.FE.XML.Models.Externo.Retenciones;
using DGII.FE.XML.Models.Externo.SubDescuentos;
using DGII.FE.XML.Models.Externo.SubRecargos;

namespace DGII.FE.XML.Models.Externo.Detalles
{
    public class DetalleFacturaCreditoFiscalElectronica : Detalle
    {
        public DetalleFacturaCreditoFiscalElectronica() { }
        public DetalleFacturaCreditoFiscalElectronica(
            string nombre,
            string descripcion,
            decimal precioUnitario,
            int? bienServicio,
            int? tipoITBIS,
            Retencion retencion,
            decimal cantidad,
            int? unidadMedida,
            DateTime? fechaElaboracion,
            DateTime? fechaVencimiento,
            decimal? gradosAlcohol,
            decimal? precioUnitarioReferencia,
            string unidadReferencia,
            decimal? cantidadReferencia,
            List<SubcantidadItem> subcantidadItems) : base(
                nombre: nombre,
                descripcion: descripcion,
                precioUnitario: precioUnitario,
                bienServicio: bienServicio,
                tipoITBIS: tipoITBIS,
                cantidad: cantidad,
                unidadMedida: unidadMedida,
                fechaElaboracion: fechaElaboracion,
                fechaVencimiento: fechaVencimiento,
                gradosAlcohol: gradosAlcohol,
                precioUnitarioReferencia: precioUnitarioReferencia,
                unidadReferencia: unidadReferencia,
                cantidadReferencia: cantidadReferencia,
                subcantidadItems: subcantidadItems)
        {
            Nombre = nombre;
            Descripcion = descripcion;
            PrecioUnitario = precioUnitario;
            BienServicio = bienServicio;
            TipoITBIS = tipoITBIS;
            Cantidad = cantidad;
            Retencion = retencion;
            UnidadMedida = unidadMedida;
        }

        public Retencion Retencion { get; set; }
        public List<DetalleImpuestoAdicional> ImpuestosAdicionales { get; set; }
        public List<SubDescuento> SubDescuentos { get; set; }
        public List<SubRecargo> SubRecargos { get; set; }
    }
}
