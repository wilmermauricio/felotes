using DGII.FE.Models.Interno;
using DGII.FE.XML.Enum;
using DGII.FE.XML.Models.Externo.Compradores;
using DGII.FE.XML.Models.Externo.Detalles;
using DGII.FE.XML.Models.Externo.FormasPago;
using DGII.FE.XML.Models.Externo.InformacionesReferencia;

namespace DGII.FE.XML.Models.Externo.Comprobantes
{
    public class NotaCreditoElectronica : Comprobante
    {
        private bool _articulosIncluyenITBIS;
        public NotaCreditoElectronica() { }
        public NotaCreditoElectronica(
            string secuencia,
            Emisor emisor,
            int? tipoPago,
            bool? articulosIncluyenItbis,
            DateTime fechaEmision,
            List<DetalleNotaCedito> detalles,
            DateTime? fechaLimitePago,
            InformacionReferencia informacionReferencia,
            bool notCredito,
            int? tipoIngreso,
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
            TipoIngreso = tipoIngreso;
            Secuencia = secuencia;
            Emisor = emisor;
            TipoPago = tipoPago;
            FechaEmision = fechaEmision;
            TipoMoneda = tipoMoneda;
            TipoCambio = tipoCambio;
            Detalles = detalles;
            FechaLimitePago = fechaLimitePago;
            ArticulosIncluyenITBIS = articulosIncluyenItbis;
            FormasPago = formasPago;
            EsNotaCredito = notCredito;
            InformacionReferencia = informacionReferencia;
        }
        public List<FormaDePago> FormasPago { get; set; }
        public bool? ArticulosIncluyenITBIS
        {
            get; set;
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
        public DateTime? FechaLimitePago { get; set; }
        public Comprador Comprador { get; set; }
        public List<DetalleNotaCedito> Detalles { get; set; }

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
        public InformacionesAdicionales InformacionesAdicionales { get; set; }
        public InformacionReferencia InformacionReferencia { get; set; }
        public bool EsNotaCredito { get; set; }
        public int? IndicadorNotaCredito { get; set; }


        internal static FacturaInterna ToFacturaInterna(NotaCreditoElectronica comprobante)
        {
            var facturaInterna = new FacturaInterna
            {
                BancoPago = comprobante.BancoPago,
                NumeroCuentaPago = comprobante.NumeroCuentaPago,
                TerminoPago = comprobante.TerminoPago,
                DescuentosORecargos = DescuentoORecargo.ToDescuentoORecargoInterno(comprobante.DescuentosORecargos),
                InformacionesAdicionales = InformacionesAdicionales.ToInformacionesAdicionalesInterno(comprobante.InformacionesAdicionales),
                TipoIngresos = comprobante.TipoIngreso,
                InformacionReferencia = InformacionReferencia.ToFacturaInterna(comprobante.InformacionReferencia),
                IndicadorNotaCredito = comprobante.IndicadorNotaCredito ?? (comprobante.EsNotaCredito ? 1 : 0),
                MontoPeriodo = comprobante.MontoPeriodo,
                ValorPagar = comprobante.ValorPagar,
                UsaOtraMoneda = comprobante.TipoCambio == null ? false : true,
                TipoCambio = comprobante.TipoCambio,
                TipoMoneda = comprobante.TipoMoneda,
                TipoECF = TipoComprobante.NotaCredito,
                ENCF = comprobante.Secuencia,
                FechaEmision = comprobante.FechaEmision,

                IndicadorMontoGravado = comprobante.ArticulosIncluyenITBIS == null ? null :
                (comprobante.ArticulosIncluyenITBIS == true ? 1 : 0),
                TipoPago = (int)comprobante.TipoPago,
                FechaLimitePago = comprobante.FechaLimitePago,
                Emisor = Emisor.ToEmisorInterno(comprobante.Emisor),
                Comprador = Comprador.ToCompradorInterno(comprobante.Comprador),
                Detalles = Detalle.ToDetallesInternos(comprobante.Detalles, comprobante.TipoCambio, comprobante.BaseOtraMoneda),
                FormasPago = FormaDePago.ToFormasPagoInterno(comprobante.FormasPago)
            };
            return facturaInterna;
        }
    }
}
