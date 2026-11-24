using DGII.FE.XML.Models.Interno;

namespace ComprobantesElectronicos.Data.Entities
{
    public class EncabezadoInterno
    {
        public decimal? Version { get; set; }
        public string? TipoECF { get; set; }
        public string? ENCF { get; set; }
        public DateTime? FechaLimitePago { get; set; }
        public DateTime? FechaVencimientoSecuencia { get; set; }
        public int? IndicadorNotaCredito { get; set; }
        public int? IndicadorEnvioDiferido { get; set; }
        public int? IndicadorMontoGravado { get; set; }
        public int? TipoIngresos { get; set; }
        public int? TipoPago { get; set; }
        public string? NumeroCuentaPago { get; set; }
        public string? BancoPago { get; set; }
        public DateTime? FechaDesde { get; set; } = null;
        public DateTime? FechaHasta { get; set; } = null;
        public DateTime? FechaEmision { get; set; } = null;
        public string? TotalPaginas { get; set; }
        public List<DetalleInterno>? Detalles { get; set; }
        public CompradorInterno? Comprador { get; set; }
        public List<FormaPagoInterno>? FormasPago { get; set; }
        public EmisorInterno? Emisor { get; set; }
        public InformacionesAdicionalesInterno? InformacionesAdicionales { get; set; }
        public TransporteInterno? Transporte { get; set; }
        public TotalInterno? Total { get; set; }
        public OtraMonedaInterno? OtraMoneda { get; set; }
        public List<PaginaInterno>? Paginas { get; set; }
        public InformacionReferenciaInterno? InformacionReferencia { get; set; }
        public List<SubTotalInterno>? SubTotales { get; set; }
        public List<DescuentoORecargoInterno>? DescuentosORecargos { get; set; }
    }
}
