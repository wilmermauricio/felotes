namespace DGII.FE.XML.Models.Externo.Comprobantes
{
    public abstract class Comprobante
    {
        private decimal? _tipoCambio;
        private string _tipoMoneda;
        protected int? _tipoPago;
        public Comprobante() { }
        public Comprobante(
            string secuencia,
            Emisor emisor,
            DateTime fechaEmision,
            string? tipoMoneda = null,
            decimal? tipoCambio = null,
            decimal? montoPeriodo = null,
            decimal? valorPagar = null,
            decimal? montoNoFacturable = null,
            List<DescuentoORecargo> descuentosORecargos = null,
            string terminoPago = null,
            string numeroCuentaPago = null,
            string? bancoPago = null)
        {
            Secuencia = secuencia;
            Emisor = emisor;
            FechaEmision = fechaEmision;
            TipoMoneda = tipoMoneda;
            TipoCambio = tipoCambio;
            MontoPeriodo = montoPeriodo;
            ValorPagar = valorPagar;
            MontoNoFacturable = montoNoFacturable;
            DescuentosORecargos = descuentosORecargos;
            TerminoPago = terminoPago;
            NumeroCuentaPago = numeroCuentaPago;
            BancoPago = bancoPago;
        }
        public string Secuencia { get; set; }
        public decimal? MontoPeriodo { get; set; }
        public bool BaseOtraMoneda { get; set; }
        public decimal? MontoNoFacturable { get; set; }
        public decimal? ValorPagar { get; set; }
        public Emisor Emisor { get; set; }
        public DateTime FechaEmision { get; set; }
        public string TerminoPago { get; set; }
        public string NumeroCuentaPago { get; set; }
        public string? BancoPago { get; set; }
        public List<DescuentoORecargo> DescuentosORecargos { get; set; }
        public decimal? TipoCambio
        {
            get => _tipoCambio;
            set
            {
                if (!string.IsNullOrWhiteSpace(TipoMoneda) && value == null)
                {
                    throw new InvalidOperationException("Si se especifica 'TipoMoneda', también debe especificarse 'TipoCambio'.");
                }
                _tipoCambio = value;
            }
        }
        public string TipoMoneda
        {
            get => _tipoMoneda;
            set
            {
                if (string.IsNullOrWhiteSpace(value) && TipoCambio.HasValue)
                {
                    throw new InvalidOperationException("No se puede especificar 'TipoCambio' sin especificar 'TipoMoneda'.");
                }
                _tipoMoneda = value;
            }
        }
    }
}
