using DGII.FE.Models.Interno;
using DGII.FE.XML.Enum;
using DGII.FE.XML.Models.Externo.Compradores;
using DGII.FE.XML.Models.Externo.Detalles;
using DGII.FE.XML.Models.Externo.FormasPago;
using DGII.FE.XML.Models.Externo.Transportes;

namespace DGII.FE.XML.Models.Externo.Comprobantes
{
    public class FacturaCreditoFiscalElectronica : Comprobante
    {
        private DateTime? _fechaVencimientoSecuencia;
        public FacturaCreditoFiscalElectronica() { }
        public FacturaCreditoFiscalElectronica(
            string secuencia,
            Emisor emisor,
            int? tipoPago,
            Comprador comprador,
            DateTime? fechaLimitePago,
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
            FechaLimitePago = fechaLimitePago;
            Comprador = comprador;
            FormasPago = formasPago;
        }


        public List<FormaDePago> FormasPago { get; set; }
        public int? TipoPago
        {
            get => _tipoPago;
            set
            {
                if (value == null)
                {
                    throw new ArgumentOutOfRangeException(nameof(TipoPago),
                        "TipoPago no puede ser nulo. Valores válidos: 1 (Contado), 2 (Crédito), 3 (Gratuito).");
                }

                if (value is < 1 or > 3)
                {
                    throw new ArgumentOutOfRangeException(nameof(TipoPago),
                        $"El valor {value} no es válido para TipoPago. Valores válidos: 1 (Contado), 2 (Crédito), 3 (Gratuito).");
                }

                //if (value == 2)
                //{
                //    if (FechaLimitePago is null)
                //    {
                //        throw new InvalidOperationException("FechaLimitePago es obligatoria para facturas a crédito (TipoPago = 2).");
                //    }

                //    if (FechaLimitePago < FechaEmision)
                //    {
                //        throw new InvalidOperationException("FechaLimitePago debe ser mayor o igual a FechaEmision para facturas a crédito.");
                //    }
                //}

                _tipoPago = value;
            }
        }

        public DateTime FechaVencimientoSecuencia { get; set; }

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

        private Comprador _comprador;

        public Comprador Comprador
        {
            get => _comprador;
            set
            {
                if (value is CompradorExtranjero)
                {
                    throw new ArgumentException("No se permite un comprador extranjero para el tipo de comprobante 31.");
                }
                _comprador = value;
            }
        }
        public List<DetalleFacturaCreditoFiscalElectronica> Detalles { get; set; }
        public Transporte Transporte { get; set; }

        public InformacionesAdicionales InformacionesAdicionales { get; set; }
        internal static FacturaInterna ToFacturaInterna(FacturaCreditoFiscalElectronica comprobante)
        {
            var facturaInterna = new FacturaInterna
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
                TipoECF = TipoComprobante.FacturaCreditoFiscal,
                ENCF = comprobante.Secuencia,
                FechaEmision = comprobante.FechaEmision,
                FechaVencimientoSecuencia = comprobante.FechaVencimientoSecuencia,
                IndicadorMontoGravado = comprobante.ArticulosIncluyenITBIS is null ? null : (comprobante.ArticulosIncluyenITBIS.Value ? 1 : 0),
                TipoIngresos = (int)comprobante.TipoIngreso,
                TipoPago = (int)comprobante.TipoPago,
                FechaLimitePago = comprobante.FechaLimitePago,
                Emisor = Emisor.ToEmisorInterno(comprobante.Emisor),
                Comprador = Comprador.ToCompradorInterno(comprobante.Comprador),
                //   Detalles = Detalle.ToDetallesInternos(comprobante.Detalles, comprobante.TipoCambio),
                Detalles = Detalle.ToDetallesInternos(comprobante.Detalles, comprobante.TipoCambio, comprobante.BaseOtraMoneda),
                FormasPago = FormaDePago.ToFormasPagoInterno(comprobante.FormasPago),

            };
            return facturaInterna;
        }
    }
}
