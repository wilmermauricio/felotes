using DGII.FE.Models.Interno;

namespace DGII.FE.XML.Models.Externo
{
    public class FacturaFirmada
    {
        public string XML { get; set; }
        public string RFCE { get; set; }
        public required string RncEmisor { get; set; }
        public required string RncComprador { get; set; }
        public required string Secuencia { get; set; }
        public required string FechaEmision { get; set; }
        public required decimal MontoTotal { get; set; }
        public required string FechaFirma { get; set; }
        public string CodigoSeguridad { get; set; }
        public required string TipoComprobante { get; set; }
        public bool IsFC { get; set; }
        public FacturaInterna FacturaInterna { get; set; }
    }
}
