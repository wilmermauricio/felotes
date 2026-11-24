namespace DGII.FE.XML.Models.Externo.Retenciones
{
    public class Retencion
    {
        public Retencion() { }

        public Retencion(int? agentePercepcionRetencion, decimal? montoITBISRetenido, decimal? montoISRRetenido)
        {
            AgentePercepcionRetencion = agentePercepcionRetencion;
            MontoISRRetenido = montoISRRetenido;
            MontoITBISRetenido = montoITBISRetenido;
        }
        public int? AgentePercepcionRetencion { get; set; }
        public decimal? MontoITBISRetenido { get; set; }
        public decimal? MontoISRRetenido { get; set; }
    }
}
