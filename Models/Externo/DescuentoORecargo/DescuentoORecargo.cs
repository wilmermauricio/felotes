using DGII.FE.XML.Models.Interno;

namespace DGII.FE.XML.Models.Externo
{
    public class DescuentoORecargo
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

        public static List<DescuentoORecargoInterno> ToDescuentoORecargoInterno(List<DescuentoORecargo> descuentoORecargos)
        {
            return descuentoORecargos?.Select(x => new DescuentoORecargoInterno
            {
                DescripcionDescuentooRecargo = x.DescripcionDescuentooRecargo,
                IndicadorFacturacionDescuentooRecargo = x.IndicadorFacturacionDescuentooRecargo,
                MontoDescuentooRecargo = x.MontoDescuentooRecargo,
                MontoDescuentooRecargoOtraMoneda = x.MontoDescuentooRecargoOtraMoneda,
                ValorDescuentooRecargo = x.ValorDescuentooRecargo,
                IndicadorNorma1007 = x.IndicadorNorma1007,
                NumeroLinea = x.NumeroLinea,
                TipoAjuste = x.TipoAjuste,
                TipoValor = x.TipoValor
            }).ToList();
        }
    }
}
