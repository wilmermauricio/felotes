
using DGII.FE.Models.Interno;
using DGII.FE.XML.Enum;
using DGII.FE.XML.Models.Externo.Compradores;
using DGII.FE.XML.Models.Externo.Detalles;
using DGII.FE.XML.Models.Externo.FormasPago;
using DGII.FE.XML.Models.Externo.Transportes;

namespace DGII.FE.XML.Models.Externo.Comprobantes
{
    public class FacturaConsumoElectronica : Comprobante
    {
        public FacturaConsumoElectronica() { }
        public FacturaConsumoElectronica(
            string secuencia,
            Emisor emisor,
            int? tipoPago,
            bool articulosIncluyenITBIS,
            Comprador comprador,
            DateTime? fechaLimitePago,
            int? tipoIngreso,
            DateTime fechaEmision,
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
            Secuencia = secuencia;
            Emisor = emisor;
            TipoPago = tipoPago;
            FechaEmision = fechaEmision;
            TipoMoneda = tipoMoneda;
            TipoCambio = tipoCambio;
            ArticulosIncluyenITBIS = articulosIncluyenITBIS;
            Comprador = comprador;
            FechaLimitePago = fechaLimitePago;
            TipoIngreso = tipoIngreso;
            FormasPago = formasPago;
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
        public bool? ArticulosIncluyenITBIS { get; set; }
        private int? _tipoIngreso;

        public int? TipoIngreso
        {
            get => _tipoIngreso;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(TipoIngreso), "El tipo de ingreso no puede ser nulo.");
                }
                _tipoIngreso = value;
            }
        }

        public DateTime? FechaLimitePago
        {
            get; set;
        }
        public Comprador Comprador { get; set; }
        public List<DetalleFacturaConsumoElectronica> Detalles { get; set; }

        public List<FormaDePago> FormasPago { get; set; }
        public Transporte Transporte { get; set; }
        public InformacionesAdicionales InformacionesAdicionales { get; set; }

        internal static FacturaInterna ToFacturaInterna(FacturaConsumoElectronica comprobante)
        {
            var facturaIterna = new FacturaInterna
            {
                BancoPago = comprobante.BancoPago,
                NumeroCuentaPago = comprobante.NumeroCuentaPago,
                TerminoPago = comprobante.TerminoPago,
                DescuentosORecargos = DescuentoORecargo.ToDescuentoORecargoInterno(comprobante.DescuentosORecargos),
                InformacionesAdicionales = InformacionesAdicionales.ToInformacionesAdicionalesInterno(comprobante.InformacionesAdicionales),
                Transporte = Transporte.ToTransporteInterno(comprobante.Transporte),
                MontoPeriodo = comprobante.MontoPeriodo,
                ValorPagar = comprobante.ValorPagar,
                TipoCambio = comprobante.TipoCambio,
                TipoMoneda = comprobante.TipoMoneda,
                UsaOtraMoneda = comprobante.TipoCambio == null ? false : true,
                TipoECF = TipoComprobante.FacturaConsumo,
                ENCF = comprobante.Secuencia,
                IndicadorMontoGravado = comprobante.ArticulosIncluyenITBIS == null ? null :
                (comprobante.ArticulosIncluyenITBIS == true ? 1 : 0),
                TipoIngresos = (int)comprobante.TipoIngreso,
                TipoPago = (int)comprobante.TipoPago,
                FechaLimitePago = comprobante.FechaLimitePago,
                FechaEmision = comprobante.FechaEmision,
                Emisor = Emisor.ToEmisorInterno(comprobante.Emisor),
                Comprador = Comprador.ToCompradorInterno(comprobante.Comprador),
                Detalles = Detalle.ToDetallesInternos(comprobante.Detalles, comprobante.TipoCambio, comprobante.BaseOtraMoneda),
                FormasPago = FormaDePago.ToFormasPagoInterno(comprobante.FormasPago)
            };
            return facturaIterna;
        }
    }
}
