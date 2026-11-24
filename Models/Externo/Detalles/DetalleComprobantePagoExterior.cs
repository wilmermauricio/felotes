using DGII.FE.XML.Models.Externo.Retenciones;

namespace DGII.FE.XML.Models.Externo.Detalles
{
    public class DetalleComprobantePagoExterior : Detalle
    {
        public DetalleComprobantePagoExterior() { }
        public DetalleComprobantePagoExterior(
            string nombre,
            string descripcion,
            decimal precioUnitario,
            Retencion retencion,
            int? bienServicio,
            int? tipoITBIS,
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
        public required Retencion Retencion { get; set; }
    }
}
