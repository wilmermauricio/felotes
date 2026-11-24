namespace DGII.FE.XML.Models.Interno
{
    public class DescuentoORecargoInterno
    {
        public string? NumeroLinea { get; set; }
        public string? TipoAjuste { get; set; }
        public string? IndicadorNorma1007 { get; set; }
        public string? DescripcionDescuentooRecargo { get; set; }
        public string? TipoValor { get; set; }
        public decimal? ValorDescuentooRecargo { get; set; }
        public decimal? MontoDescuentooRecargo { get; set; }
        public decimal? MontoDescuentooRecargoOtraMoneda { get; set; }
        public int? IndicadorFacturacionDescuentooRecargo { get; set; }
    }
}
