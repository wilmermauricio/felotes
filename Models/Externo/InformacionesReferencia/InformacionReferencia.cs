using DGII.FE.XML.Models.Interno;

namespace DGII.FE.XML.Models.Externo.InformacionesReferencia
{
    public class InformacionReferencia
    {
        public string? NCFModificado { get; set; }
        public string? RNCOtroContribuyente { get; set; }
        public DateTime? FechaNCFModificado { get; set; }
        public int? CodigoModificacion { get; set; }
        public string RazonModificacion { get; set; }

        internal static InformacionReferenciaInterno ToFacturaInterna(InformacionReferencia infoRef)
        {
            var informacionReferencia = new InformacionReferenciaInterno
            {
                CodigoModificacion = infoRef?.CodigoModificacion,
                FechaNCFModificado = infoRef?.FechaNCFModificado,
                NCFModificado = infoRef?.NCFModificado,
                RNCOtroContribuyente = infoRef?.RNCOtroContribuyente,
                RazonModificacion = infoRef?.RazonModificacion
            };
            return informacionReferencia;
        }
    }
}
