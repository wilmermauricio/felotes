using DGII.FE.Models.Interno;
using DGII.FE.XML.Enum;
using DGII.FE.XML.Models.Externo.Compradores;
using DGII.FE.XML.Models.Externo.Detalles;
using DGII.FE.XML.Models.Externo.FormasPago;
using DGII.FE.XML.Models.Externo.Transportes;

namespace DGII.FE.XML.Models.Externo.Comprobantes
{
    public class ComprobanteRegimenesEspeciales : Comprobante
    {
        public ComprobanteRegimenesEspeciales() { }
        public ComprobanteRegimenesEspeciales(
            string secuencia,
            Emisor emisor,
            int? tipoPago,
            DateTime fechaEmision,
            DateTime? fechaVencimientoSecuencia,
            DateTime? fechaLimitePago,
            List<DetalleComprobanteRegimenesEspeciales> detalles,
            Comprador comprador,
            string? tipoMoneda = null,
            decimal? tipoCambio = null,
            List<FormaDePago> formasPago = null,
            decimal? montoPeriodo = null,
            decimal? valorPagar = null,
            decimal? montoNoFacturable = null,
            List<DescuentoORecargo> descuentosORecargos = null,
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
                descuentosORecargos: descuentosORecargos,
                terminoPago: terminoPago,
                numeroCuentaPago: numeroCuentaPago,
                bancoPago: bancoPago)
        {
            Comprador = comprador;
            Secuencia = secuencia;
            Emisor = emisor;
            TipoPago = tipoPago;
            FechaEmision = fechaEmision;
            TipoMoneda = tipoMoneda;
            FechaLimitePago = fechaLimitePago;
            TipoCambio = tipoCambio;
            FechaVencimientoSecuencia = fechaVencimientoSecuencia;
            Detalles = detalles;
            FormasPago = formasPago;
        }

        private DateTime? _fechaVencimientoSecuencia;
        public List<FormaDePago> FormasPago { get; set; }
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

        public DateTime? FechaLimitePago { get; set; }

        public int? TipoIngreso { get; set; }

        public Comprador Comprador { get; set; }
        public List<DetalleComprobanteRegimenesEspeciales> Detalles { get; set; }
        public Transporte Transporte { get; set; }
        public InformacionesAdicionales InformacionesAdicionales { get; set; }

        internal static FacturaInterna ToFacturaInterna(ComprobanteRegimenesEspeciales comprobante)
        {
            var facturaInterna = new FacturaInterna
            {
                BancoPago = comprobante.BancoPago,
                NumeroCuentaPago = comprobante.NumeroCuentaPago,
                TerminoPago = comprobante.TerminoPago,
                DescuentosORecargos = DescuentoORecargo.ToDescuentoORecargoInterno(comprobante.DescuentosORecargos),
                InformacionesAdicionales = InformacionesAdicionales.ToInformacionesAdicionalesInterno(comprobante?.InformacionesAdicionales),
                Transporte = Transporte.ToTransporteInterno(comprobante.Transporte),
                MontoPeriodo = comprobante.MontoPeriodo,
                ValorPagar = comprobante.ValorPagar,
                TipoCambio = comprobante.TipoCambio,
                TipoMoneda = comprobante.TipoMoneda,
                UsaOtraMoneda = comprobante.TipoCambio == null ? false : true,
                TipoECF = TipoComprobante.ComprobanteRegimenesEspeciales,
                ENCF = comprobante.Secuencia,
                FechaVencimientoSecuencia = comprobante.FechaVencimientoSecuencia,
                TipoIngresos = comprobante.TipoIngreso,
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
