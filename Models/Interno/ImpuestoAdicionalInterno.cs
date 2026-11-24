namespace DGII.FE.XML.Models.Interno
{
    public class ImpuestoAdicionalInterno
    {
        public string? TipoImpuesto { get; set; }
        public decimal? TasaImpuestoAdicional { get; set; }
        public decimal? MontoImpuestoSelectivoConsumoEspecifico { get; set; }
        public decimal? MontoImpuestoSelectivoConsumoAdvalorem { get; set; }
        public decimal? OtrosImpuestosAdicionales { get; set; }
    }
}
