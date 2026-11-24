namespace DGII.FE.XML.Models.Externo.DetalleImpuestosAdicionales
{
    public class DetalleImpuestoAdicional
    {
        public decimal? TasaImpuestoAdicional { get; set; }
        public string? TipoImpuesto { get; set; }
        public decimal? MontoImpuestoSelectivoConsumoEspecifico { get; set; }
        public decimal? MontoImpuestoSelectivoConsumoAdvalorem { get; set; }
        public decimal? OtrosImpuestosAdicionales { get; set; }
    }
}
