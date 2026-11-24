using DGII.FE.Models.Interno;
using DGII.FE.XML.Enum;
using DGII.FE.XML.Models.Externo.Compradores;
using DGII.FE.XML.Models.Externo.Detalles;
using DGII.FE.XML.Models.Externo.FormasPago;
using DGII.FE.XML.Models.Externo.Transportes;

namespace DGII.FE.XML.Models.Externo.Comprobantes
{
    public class ComprobantePagoExterior : Comprobante
    {
        public ComprobantePagoExterior() { }
        public ComprobantePagoExterior(
            string secuencia,
            Emisor emisor,
            int? tipoPago,
            Comprador comprador,
            List<DetalleComprobantePagoExterior> detalles,
            DateTime fechaEmision,
            DateTime? fechaLimitePago,
            string? tipoMoneda = null,
            decimal? tipoCambio = null,
            List<FormaDePago> formasPago = null,
            decimal? montoPeriodo = null,
            decimal? valorPagar = null,
            decimal? montoNoFacturable = null,
            string terminoPago = null,
            string numeroCuentaPago = null,
            string? bancoPago = null) : base(
                secuencia: secuencia,
                emisor: emisor,
                fechaEmision: fechaEmision,
                tipoMoneda: tipoMoneda,
                tipoCambio: tipoCambio,
                montoPeriodo: montoPeriodo,
                valorPagar: valorPagar,
                montoNoFacturable: montoNoFacturable,
                terminoPago: terminoPago,
                numeroCuentaPago: numeroCuentaPago,
                bancoPago: bancoPago)
        {
            Secuencia = secuencia;
            Emisor = emisor;
            FechaEmision = fechaEmision;
            TipoMoneda = tipoMoneda;
            TipoCambio = tipoCambio;
            TipoPago = tipoPago;
            FechaLimitePago = fechaLimitePago;
            Comprador = comprador;
            Detalles = detalles;
            FormasPago = formasPago;
        }

        private DateTime? _fechaVencimientoSecuencia;

        public List<FormaDePago> FormasPago { get; set; }
        public DateTime? FechaVencimientoSecuencia
        {
            get => _fechaVencimientoSecuencia;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(FechaVencimientoSecuencia), "La fecha de vencimiento no puede ser nula.");
                }
                _fechaVencimientoSecuencia = value;
            }
        }
        public int? TipoPago
        {
            get => _tipoPago;
            set
            {

                if (value == null)
                {
                    throw new ArgumentOutOfRangeException(nameof(TipoPago), $"El valor {value} no es válido para TipoPago. Valores válidos: 1 (Contado), 2 (Crédito), 3 (Gratuito).");
                }

                _tipoPago = value;
            }
        }
        public DateTime? FechaLimitePago
        {
            get; set;
        }
        public Comprador Comprador { get; set; }
        public List<DetalleComprobantePagoExterior> Detalles { get; set; }
        public Transporte Transporte { get; set; }

        internal static FacturaInterna ToFacturaInterna(ComprobantePagoExterior comprobante)
        {
            var facturaInterna = new FacturaInterna
            {
                BancoPago = comprobante.BancoPago,
                NumeroCuentaPago = comprobante.NumeroCuentaPago,
                TerminoPago = comprobante.TerminoPago,
                DescuentosORecargos = DescuentoORecargo.ToDescuentoORecargoInterno(comprobante.DescuentosORecargos),
                Transporte = Transporte.ToTransporteInterno(comprobante.Transporte),
                MontoPeriodo = comprobante.MontoPeriodo,
                ValorPagar = comprobante.ValorPagar,
                TipoCambio = comprobante.TipoCambio,
                TipoMoneda = comprobante.TipoMoneda,
                UsaOtraMoneda = comprobante.TipoCambio == null ? false : true,
                TipoECF = TipoComprobante.ComprobantePagosExterior,
                ENCF = comprobante.Secuencia,
                FechaVencimientoSecuencia = comprobante.FechaVencimientoSecuencia,
                TipoPago = comprobante.TipoPago,
                FechaLimitePago = comprobante.FechaLimitePago,
                FechaEmision = comprobante.FechaEmision,
                Emisor = Emisor.ToEmisorInterno(comprobante.Emisor),
                Comprador = Comprador.ToCompradorInterno(comprobante.Comprador),
                Detalles = Detalle.ToDetallesInternos(comprobante.Detalles, comprobante.TipoCambio, comprobante.BaseOtraMoneda),
                FormasPago = FormaDePago.ToFormasPagoInterno(comprobante.FormasPago)
            };
            return facturaInterna;
        }
    }
}
