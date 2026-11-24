using DGII.FE.Models.Interno;
using DGII.FE.XML.Enum;
using DGII.FE.XML.Models.Externo.Detalles;

namespace DGII.FE.XML.Models.Externo.Comprobantes
{
    public class ComprobanteGastosMenores : Comprobante
    {
        public ComprobanteGastosMenores() { }
        public ComprobanteGastosMenores(
          string secuencia,
          Emisor emisor,
          int? tipoPago,
          DateTime fechaEmision,
          List<DetalleComprobanteGastosMenores> detalles,
          DateTime? fechaVencimientoSecuencia,
          string? tipoMoneda = null,
          decimal? tipoCambio = null,
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
            TipoPago = tipoPago;
            FechaEmision = fechaEmision;
            TipoMoneda = tipoMoneda;
            TipoCambio = tipoCambio;
            Detalles = detalles;
            FechaVencimientoSecuencia = fechaVencimientoSecuencia;
        }
        private DateTime? _fechaVencimientoSecuencia;
        public int? TipoPago { get; set; }

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
        public List<DetalleComprobanteGastosMenores> Detalles { get; set; }

        internal static FacturaInterna ToFacturaInterna(ComprobanteGastosMenores comprobante)
        {
            var facturaInterna = new FacturaInterna
            {
                BancoPago = comprobante.BancoPago,
                NumeroCuentaPago = comprobante.NumeroCuentaPago,
                TerminoPago = comprobante.TerminoPago,
                MontoPeriodo = comprobante.MontoPeriodo,
                ValorPagar = comprobante.ValorPagar,
                TipoCambio = comprobante.TipoCambio,
                TipoMoneda = comprobante.TipoMoneda,
                UsaOtraMoneda = comprobante.TipoCambio == null ? false : true,
                TipoECF = TipoComprobante.ComprobanteGastosMenores,
                ENCF = comprobante.Secuencia,
                FechaEmision = comprobante.FechaEmision,
                FechaVencimientoSecuencia = comprobante.FechaVencimientoSecuencia,
                TipoPago = comprobante.TipoPago,
                Emisor = Emisor.ToEmisorInterno(comprobante.Emisor),
                Detalles = Detalle.ToDetallesInternos(comprobante.Detalles, comprobante.TipoCambio, comprobante.BaseOtraMoneda)
            };
            return facturaInterna;
        }
    }
}
