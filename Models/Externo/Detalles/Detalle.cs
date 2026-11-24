using DGII.FE.XML.Models.Externo.CodigosItems;
using DGII.FE.XML.Models.Externo.DetalleImpuestosAdicionales;
using DGII.FE.XML.Models.Externo.OtrasMonedasDetalle;
using DGII.FE.XML.Models.Externo.Retenciones;
using DGII.FE.XML.Models.Externo.SubDescuentos;
using DGII.FE.XML.Models.Externo.SubRecargos;
using DGII.FE.XML.Models.Interno;

namespace DGII.FE.XML.Models.Externo.Detalles
{
    public class Detalle
    {

        private int? _tipoITBIS;
        private string _nombre;
        private int? _bienServicio;
        private decimal _cantidad;
        public Detalle() { }
        public Detalle(
            string nombre,
            string descripcion,
            decimal precioUnitario,
            int? bienServicio,
            int? tipoITBIS,
            decimal cantidad,
            int? unidadMedida,
            DateTime? fechaElaboracion,
            DateTime? fechaVencimiento,
            decimal? gradosAlcohol,
            decimal? precioUnitarioReferencia,
            string unidadReferencia,
            decimal? cantidadReferencia,
            OtraMonedaDetalle otraMonedaDetalle = null,
            List<CodigosItem> codigosItems = null,
            List<SubcantidadItem> subcantidadItems = null)
        {
            Nombre = nombre;
            Descripcion = descripcion;
            PrecioUnitario = precioUnitario;
            BienServicio = bienServicio;
            TipoITBIS = tipoITBIS;
            Cantidad = cantidad;
            UnidadMedida = unidadMedida;
            OtraMonedaDetalle = otraMonedaDetalle;
            CodigosItems = codigosItems;
            FechaElaboracion = fechaElaboracion;
            FechaVencimiento = fechaVencimiento;
            PrecioUnitarioReferencia = precioUnitarioReferencia;
            GradosAlcohol = gradosAlcohol;
            UnidadReferencia = unidadReferencia;
            CantidadReferencia = cantidadReferencia;
            SubcantidadItems = subcantidadItems;
        }
        public decimal? GradosAlcohol { get; set; }
        public decimal? PrecioUnitarioReferencia { get; set; }
        public DateTime? FechaElaboracion { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string Nombre
        {
            get => _nombre;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentNullException("El valor de NombreItem es requerido y no puede ser null, vacío o solo espacios en blanco.", nameof(Nombre));
                }

                if (value.Length < 1 || value.Length > 80)
                {
                    throw new ArgumentException("El valor de NombreItem debe tener entre 1 y 80 caracteres.", nameof(Nombre));
                }

                _nombre = value;
            }
        }
        public string? Descripcion { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Cantidad { get; set; }

        public int? TipoITBIS
        {
            get => _tipoITBIS;
            set
            {
                if (value == null)
                {
                    throw new ArgumentException("El valor de IndicadorFacturacion no puede ser null o no definido.", nameof(TipoITBIS));
                }

                _tipoITBIS = value;
            }
        }
        public int? BienServicio
        {
            get => _bienServicio;
            set
            {
                if (value != 1 && value != 2)
                {
                    throw new ArgumentException("El valor de IndicadorBienoServicio debe ser 1 (Bien) o 2 (Servicio).", nameof(BienServicio));
                }
                _bienServicio = value;
            }
        }

        private int? _unidadMedida;

        public int? UnidadMedida
        {
            get => _unidadMedida;
            set
            {
                if (value is not null)
                {
                    if ((value < 1 || value > 62))
                    {
                        throw new ArgumentOutOfRangeException(nameof(UnidadMedida),
                            $"El valor {value} no es válido para UnidadMedida. Valores válidos: 1 - 62.");
                    }
                }

                _unidadMedida = value;
            }
        }


        public OtraMonedaDetalle OtraMonedaDetalle { get; set; }
        public List<CodigosItem> CodigosItems { get; set; }
        public decimal? CantidadReferencia { get; set; }
        public string? UnidadReferencia { get; set; }
        public List<SubcantidadItem> SubcantidadItems { get; set; }



        private static List<SubDescuentoInterno> ProcesarSubDescuentos(List<SubDescuento> subDescuentos, DetalleInterno detalleInterno)
        {
            if (subDescuentos == null)
            {
                return null;
            }

            var subDescuentosInternos = new List<SubDescuentoInterno>();

            foreach (var x in subDescuentos)
            {
                var subDescuento = new SubDescuentoInterno
                {
                    MontoSubDescuento = x?.MontoSubDescuento,
                    SubDescuentoPorcentaje = x?.SubDescuentoPorcentaje,
                    TipoSubDescuento = x?.TipoSubDescuento
                };

                decimal montoSubDescuento = 0;

                if (subDescuento.TipoSubDescuento == "$" && subDescuento.MontoSubDescuento.HasValue)
                {
                    montoSubDescuento = subDescuento.MontoSubDescuento.Value;
                }
                else if (subDescuento.TipoSubDescuento == "%" && subDescuento.SubDescuentoPorcentaje.HasValue)
                {
                    montoSubDescuento = (detalleInterno.MontoItem / 100m) * subDescuento.SubDescuentoPorcentaje.Value;
                }

                detalleInterno.DescuentoMonto = (detalleInterno.DescuentoMonto ?? 0) + montoSubDescuento;

                subDescuentosInternos.Add(subDescuento);
            }

            detalleInterno.DescuentoMonto = Math.Round(detalleInterno.DescuentoMonto.GetValueOrDefault() * 1.00m, 2);

            return subDescuentosInternos;
        }

        private static List<SubRecargoInterno> ProcesarSubRecargo(List<SubRecargo> subRecargos, DetalleInterno detalleInterno)
        {
            if (subRecargos == null) return null;

            var subRecargosInterno = new List<SubRecargoInterno>();

            foreach (var x in subRecargos)
            {
                var subRecargo = new SubRecargoInterno
                {
                    MontoSubRecargo = x?.MontoSubRecargo,
                    SubRecargoPorcentaje = x?.SubRecargoPorcentaje,
                    TipoSubRecargo = x?.TipoSubRecargo
                };

                decimal montoSubRecargo = 0;

                if (subRecargo.TipoSubRecargo == "$" && subRecargo.MontoSubRecargo.HasValue)
                {
                    montoSubRecargo = subRecargo.MontoSubRecargo.Value;
                }
                else if (subRecargo.TipoSubRecargo == "%" && subRecargo.SubRecargoPorcentaje.HasValue)
                {
                    montoSubRecargo = (detalleInterno.MontoItem / 100m) * subRecargo.SubRecargoPorcentaje.Value;
                }

                detalleInterno.RecargoMonto = (detalleInterno.RecargoMonto ?? 0) + montoSubRecargo;

                subRecargosInterno.Add(subRecargo);
            }

            detalleInterno.RecargoMonto = Math.Round(detalleInterno.RecargoMonto.GetValueOrDefault(), 2);

            return subRecargosInterno;
        }




        public static OtraMonedaDetalleInterno ToOtraMonedaDetalle(
            Detalle detalle,
            decimal? tipoCambio,
            bool baseOtraMoneda)
        {
            if (tipoCambio is null && detalle.OtraMonedaDetalle is null)
            {
                return null;
            }


            if (detalle?.OtraMonedaDetalle is not null)
            {
                decimal? montoItemOtraMoneda = detalle.OtraMonedaDetalle.MontoItemOtraMoneda == null ? null :
                    Math.Round(detalle.OtraMonedaDetalle.MontoItemOtraMoneda.GetValueOrDefault(), 2);
                decimal? precioOtraMoneda = detalle.OtraMonedaDetalle.PrecioOtraMoneda == null ? null :
                    Math.Round(detalle.OtraMonedaDetalle.PrecioOtraMoneda.GetValueOrDefault(), 2);
                decimal? recargoOtraMoneda = detalle.OtraMonedaDetalle.RecargoOtraMoneda == null ? null :
                    Math.Round(detalle.OtraMonedaDetalle.RecargoOtraMoneda.GetValueOrDefault(), 2);
                decimal? descuentoOtraMoneda = detalle.OtraMonedaDetalle.DescuentoOtraMoneda == null ? null :
                    Math.Round(detalle.OtraMonedaDetalle.DescuentoOtraMoneda.GetValueOrDefault(), 2);

                if (tipoCambio is null && detalle.OtraMonedaDetalle is not null)
                {
                    return new OtraMonedaDetalleInterno
                    {
                        DescuentoOtraMoneda = descuentoOtraMoneda,
                        MontoItemOtraMoneda = montoItemOtraMoneda,
                        PrecioOtraMoneda = precioOtraMoneda,
                        RecargoOtraMoneda = recargoOtraMoneda
                    };
                }

                if (tipoCambio is not null && !baseOtraMoneda)
                {
                    precioOtraMoneda = detalle.PrecioUnitario / tipoCambio.GetValueOrDefault();

                    return new OtraMonedaDetalleInterno
                    {
                        PrecioOtraMoneda = precioOtraMoneda.GetValueOrDefault(),
                        MontoItemOtraMoneda = Math.Round(detalle.Cantidad * precioOtraMoneda.GetValueOrDefault(), 2),
                        DescuentoOtraMoneda = descuentoOtraMoneda,
                        RecargoOtraMoneda = recargoOtraMoneda
                    };
                }

            }


            if (tipoCambio is not null && detalle.OtraMonedaDetalle is null && !baseOtraMoneda)
            {
                var precioOtraMoneda = detalle.PrecioUnitario / tipoCambio.GetValueOrDefault();

                return new OtraMonedaDetalleInterno
                {
                    PrecioOtraMoneda = precioOtraMoneda,
                    MontoItemOtraMoneda = Math.Round(detalle.Cantidad * precioOtraMoneda, 2),
                };
            }

            return null;
        }
        private static RetencionInterno GetRetencion(Retencion retencion)
        {
            if (retencion is null)
            {
                return null;
            }


            return new RetencionInterno
            {
                MontoITBISRetenido = retencion?.MontoITBISRetenido,
                IndicadorAgenteRetencionoPercepcion = retencion?.AgentePercepcionRetencion,
                MontoISRRetenido = retencion?.MontoISRRetenido
            };
        }

        private static List<DetalleImpuestoAdicionalInterno> GetImpuestosAdicionales(List<DetalleImpuestoAdicional> impuestosAdicionales)
        {
            if (impuestosAdicionales == null) return null;

            return impuestosAdicionales.Select(x => new DetalleImpuestoAdicionalInterno
            {
                OtrosImpuestosAdicionales = x?.OtrosImpuestosAdicionales,
                TasaImpuestoAdicional = x?.TasaImpuestoAdicional,
                TipoImpuesto = x?.TipoImpuesto,
                MontoImpuestoSelectivoConsumoAdvalorem = x.MontoImpuestoSelectivoConsumoAdvalorem == null ? null :
                x?.MontoImpuestoSelectivoConsumoAdvalorem,
                MontoImpuestoSelectivoConsumoEspecifico = x.MontoImpuestoSelectivoConsumoEspecifico == null ? null : x?.MontoImpuestoSelectivoConsumoEspecifico
            }).ToList();
        }

        private static DetalleInterno GetDetalleInterno(Detalle detalle, int indice,
            decimal? tipoCambio, bool baseOtraMoneda)
        {
            decimal precioUnitario = 0;

            if (tipoCambio is null)
            {
                precioUnitario = detalle.PrecioUnitario;
            }
            else if (tipoCambio is not null)
            {

                if (baseOtraMoneda)
                {
                    precioUnitario = detalle.PrecioUnitario * tipoCambio.GetValueOrDefault();
                }
                else
                {
                    precioUnitario = detalle.PrecioUnitario;
                }
            }

            return new DetalleInterno
            {


                SubcantidadItems = SubcantidadItem.ToSubCantidadInterno(detalle.SubcantidadItems),
                CantidadReferencia = detalle.CantidadReferencia,
                UnidadReferencia = detalle.UnidadReferencia,
                GradosAlcohol = detalle.GradosAlcohol,
                PrecioUnitarioReferencia = detalle.PrecioUnitarioReferencia,
                NumeroLinea = indice,
                IndicadorFacturacion = detalle.TipoITBIS,
                NombreItem = detalle.Nombre,
                IndicadorBienoServicio = detalle.BienServicio.GetValueOrDefault(),
                DescripcionItem = detalle.Descripcion,
                CantidadItem = Math.Round(detalle.Cantidad, 2),
                PrecioUnitarioItem = Math.Round(precioUnitario, 4),
                MontoItem = Math.Round(detalle.Cantidad * precioUnitario, 2),
                OtraMonedaDetalle = ToOtraMonedaDetalle(detalle, tipoCambio, baseOtraMoneda),
                UnidadMedida = detalle.UnidadMedida,
                FechaElaboracion = detalle.FechaElaboracion,
                FechaVencimientoItem = detalle.FechaVencimiento,
                CodigosItems = detalle.CodigosItems?.Select(x => new CodigosItemInterno
                {
                    CodigoItem = x?.CodigoItem,
                    TipoCodigo = x?.TipoCodigo
                }).ToList()
            };
        }
        public static List<DetalleInterno> ToDetallesInternos<T>(List<T> detalles, decimal? tipoCambio, bool baseOtraMoneda) where T : Detalle
        {
            if (detalles == null) throw new ArgumentNullException(nameof(detalles));

            var detallesInternos = new List<DetalleInterno>();

            switch (detalles)
            {

                case List<DetalleComprobanteCompra> comprobantes:

                    detallesInternos = comprobantes.Select((detalle, index) =>
                       {
                           var detalleInterno = GetDetalleInterno(detalle, index + 1, tipoCambio, baseOtraMoneda);

                           detalleInterno.SubDescuentos = ProcesarSubDescuentos(detalle.SubDescuentos, detalleInterno);
                           detalleInterno.SubRecargos = ProcesarSubRecargo(detalle.SubRecargos, detalleInterno);
                           detalleInterno.MontoItem = detalleInterno.MontoItem +
                           detalleInterno.RecargoMonto.GetValueOrDefault();
                           detalleInterno.MontoItem = detalleInterno.MontoItem -
                           detalleInterno.DescuentoMonto.GetValueOrDefault();
                           detalleInterno.Retencion = GetRetencion(detalle.Retencion);

                           return detalleInterno;
                       }).ToList();



                    break;


                case List<DetalleComprobanteRegimenesEspeciales> detalleComprobanteRegimenesEspeciales:

                    detallesInternos = detalleComprobanteRegimenesEspeciales.Select((detalle, index) =>
                    {
                        var detalleInterno = GetDetalleInterno(detalle, index + 1, tipoCambio, baseOtraMoneda);

                        detalleInterno.SubDescuentos = ProcesarSubDescuentos(detalle.SubDescuentos, detalleInterno);
                        detalleInterno.MontoItem = detalleInterno.MontoItem -
                        detalleInterno.DescuentoMonto.GetValueOrDefault();
                        detalleInterno.SubRecargos = ProcesarSubRecargo(detalle.SubRecargos, detalleInterno);
                        detalleInterno.MontoItem = detalleInterno.MontoItem +
                        detalleInterno.RecargoMonto.GetValueOrDefault();
                        detalleInterno.ImpuestosAdicionales = GetImpuestosAdicionales(detalle.ImpuestosAdicionales);

                        return detalleInterno;
                    }).ToList();
                    break;

                case List<DetalleComprobanteGubernamental> detalleComprobanteGubernamental:

                    detallesInternos = detalleComprobanteGubernamental.Select((detalle, index) =>
                    {
                        var detalleInterno = GetDetalleInterno(detalle, index + 1, tipoCambio, baseOtraMoneda);

                        detalleInterno.SubDescuentos = ProcesarSubDescuentos(detalle.SubDescuentos, detalleInterno);
                        detalleInterno.MontoItem = detalleInterno.MontoItem -
                        detalleInterno.DescuentoMonto.GetValueOrDefault();
                        detalleInterno.SubRecargos = ProcesarSubRecargo(detalle.SubRecargos, detalleInterno);
                        detalleInterno.MontoItem = detalleInterno.MontoItem +
                        detalleInterno.RecargoMonto.GetValueOrDefault();
                        detalleInterno.ImpuestosAdicionales = GetImpuestosAdicionales(detalle.ImpuestosAdicionales);

                        return detalleInterno;
                    }).ToList();
                    break;


                case List<DetalleNotaCedito> detalleNotaCredito:

                    detallesInternos = detalleNotaCredito.Select((detalle, index) =>
                    {
                        var detalleInterno = GetDetalleInterno(detalle, index + 1, tipoCambio, baseOtraMoneda);

                        detalleInterno.SubDescuentos = ProcesarSubDescuentos(detalle.SubDescuentos, detalleInterno);
                        detalleInterno.MontoItem = detalleInterno.MontoItem -
                        detalleInterno.DescuentoMonto.GetValueOrDefault();
                        detalleInterno.SubRecargos = ProcesarSubRecargo(detalle.SubRecargos, detalleInterno);
                        detalleInterno.MontoItem = detalleInterno.MontoItem +
                        detalleInterno.RecargoMonto.GetValueOrDefault();
                        detalleInterno.ImpuestosAdicionales = GetImpuestosAdicionales(detalle.ImpuestosAdicionales);

                        return detalleInterno;
                    }).ToList();
                    break;


                case List<DetalleNotaDebito> detalleNotaDebito:

                    detallesInternos = detalleNotaDebito.Select((detalle, index) =>
                    {
                        var detalleInterno = GetDetalleInterno(detalle, index + 1, tipoCambio, baseOtraMoneda);

                        detalleInterno.SubDescuentos = ProcesarSubDescuentos(detalle.SubDescuentos, detalleInterno);
                        detalleInterno.MontoItem = detalleInterno.MontoItem -
                        detalleInterno.DescuentoMonto.GetValueOrDefault();
                        detalleInterno.SubRecargos = ProcesarSubRecargo(detalle.SubRecargos, detalleInterno);
                        detalleInterno.MontoItem = detalleInterno.MontoItem +
                        detalleInterno.RecargoMonto.GetValueOrDefault();
                        detalleInterno.ImpuestosAdicionales = GetImpuestosAdicionales(detalle.ImpuestosAdicionales);

                        return detalleInterno;
                    }).ToList();
                    break;


                case List<DetalleFacturaConsumoElectronica> detalleFacturaConsumoElectronica:

                    detallesInternos = detalleFacturaConsumoElectronica.Select((detalle, index) =>
                    {

                        var detalleInterno = GetDetalleInterno(detalle, index + 1, tipoCambio, baseOtraMoneda);

                        detalleInterno.SubDescuentos = ProcesarSubDescuentos(detalle.SubDescuentos, detalleInterno);
                        detalleInterno.MontoItem = detalleInterno.MontoItem -
                        detalleInterno.DescuentoMonto.GetValueOrDefault();
                        detalleInterno.SubRecargos = ProcesarSubRecargo(detalle.SubRecargos, detalleInterno);
                        detalleInterno.MontoItem = detalleInterno.MontoItem +
                        detalleInterno.RecargoMonto.GetValueOrDefault();
                        detalleInterno.ImpuestosAdicionales = GetImpuestosAdicionales(detalle.ImpuestosAdicionales);

                        return detalleInterno;
                    }).ToList();
                    break;

                case List<DetalleFacturaCreditoFiscalElectronica> detalleFacturaCreditoFiscal:

                    detallesInternos = detalleFacturaCreditoFiscal.Select((detalle, index) =>
                    {
                        var detalleInterno = GetDetalleInterno(detalle, index + 1, tipoCambio, baseOtraMoneda);

                        detalleInterno.SubDescuentos = ProcesarSubDescuentos(detalle.SubDescuentos, detalleInterno);
                        detalleInterno.MontoItem = detalleInterno.MontoItem -
                        detalleInterno.DescuentoMonto.GetValueOrDefault();
                        detalleInterno.SubRecargos = ProcesarSubRecargo(detalle.SubRecargos, detalleInterno);
                        detalleInterno.MontoItem = detalleInterno.MontoItem +
                        detalleInterno.RecargoMonto.GetValueOrDefault();
                        detalleInterno.ImpuestosAdicionales = GetImpuestosAdicionales(detalle.ImpuestosAdicionales);

                        return detalleInterno;
                    }).ToList();


                    break;

                case List<DetalleComprobantePagoExterior> detalleComprobanteElectronicoPagoExterior:

                    detallesInternos = detalleComprobanteElectronicoPagoExterior.Select((detalle, index) =>
                    {
                        //   var detalleInterno = GetDetalleInterno(detalle, index + 1, tipoCambio);

                        var detalleInterno = GetDetalleInterno(detalle, index + 1, tipoCambio, baseOtraMoneda);

                        detalleInterno.Retencion = GetRetencion(detalle.Retencion);

                        return detalleInterno;
                    }).ToList();

                    break;

                case List<DetalleComprobanteExportacion> detalleComprobanteExportacion:


                    detallesInternos = detalleComprobanteExportacion.Select((detalle, index) =>
                    {

                        var detalleInterno = GetDetalleInterno(detalle, index + 1, tipoCambio, baseOtraMoneda);

                        detalleInterno.SubDescuentos = ProcesarSubDescuentos(detalle.SubDescuentos, detalleInterno);
                        detalleInterno.MontoItem = detalleInterno.MontoItem -
                        detalleInterno.DescuentoMonto.GetValueOrDefault();
                        detalleInterno.SubRecargos = ProcesarSubRecargo(detalle.SubRecargos, detalleInterno);
                        detalleInterno.MontoItem = detalleInterno.MontoItem +
                        detalleInterno.RecargoMonto.GetValueOrDefault();

                        return detalleInterno;
                    }).ToList();

                    break;

                default:
                    detallesInternos = detalles.Select((item, index) =>
                    GetDetalleInterno(item, index + 1, tipoCambio, baseOtraMoneda)).ToList();
                    break;
            }

            return detallesInternos;
        }
    }
}
