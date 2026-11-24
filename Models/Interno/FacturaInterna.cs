using DGII.FE.XML.Models.Interno;

namespace DGII.FE.Models.Interno
{
    public class FacturaInterna
    {
        private TotalInterno? _total;
        private string? _tipoECF;
        private string? _encf;
        private List<DetalleInterno>? _detalles;
        public bool UsaOtraMoneda { get; set; }
        private OtraMonedaInterno _otraMonedaInterno = null;
        public decimal? MontoPeriodo { get; set; }
        public decimal? ValorPagar { get; set; }
        public string TipoMoneda { get; set; }
        public decimal? TipoCambio { get; set; }
        public decimal? Version { get; set; } = 1.0M;
        public string TipoECF
        {
            get => _tipoECF!;
            set
            {
                // Validar si el valor es nulo, vacío o solo espacios
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("El valor de TipoECF no puede ser null, vacío o solo espacios en blanco.", nameof(TipoECF));
                }

                // Validar si el valor cumple con el patrón de dos dígitos numéricos
                if (!System.Text.RegularExpressions.Regex.IsMatch(value, "^[0-9]{2}$"))
                {
                    throw new ArgumentException("El valor de TipoECF debe ser un número entero de dos dígitos.", nameof(TipoECF));
                }

                // Validar si el valor pertenece a los valores permitidos
                var valoresPermitidos = new HashSet<string>
        {
            "31", "32", "33", "34", "41", "43", "44", "45", "46", "47"
        };

                if (!valoresPermitidos.Contains(value))
                {
                    throw new ArgumentException($"El valor de TipoECF '{value}' no es válido. Valores permitidos: {string.Join(", ", valoresPermitidos)}.", nameof(TipoECF));
                }

                _tipoECF = value;
            }
        }
        public string ENCF
        {
            get
            {
                if (_encf == null)
                {
                    throw new InvalidOperationException("El valor de ENCF es requerido y no se ha asignado.");
                }

                return _encf;
            }
            set
            {
                // Validar si el valor es nulo, vacío o solo espacios en blanco
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("El valor de ENCF no puede ser null, vacío o solo espacios en blanco.", nameof(ENCF));
                }

                // Validar si el valor tiene exactamente 13 caracteres
                if (value.Length != 13)
                {
                    throw new ArgumentException("El valor de ENCF debe tener exactamente 13 caracteres.", nameof(ENCF));
                }

                // Validar si el valor cumple con el patrón alfanumérico
                if (!System.Text.RegularExpressions.Regex.IsMatch(value, "^[a-zA-Z0-9]{13}$"))
                {
                    throw new ArgumentException("El valor de ENCF debe ser una cadena alfanumérica de exactamente 13 caracteres.", nameof(ENCF));
                }

                _encf = value;
            }
        }

        public DateTime? FechaVencimientoSecuencia { get; set; }
        private DateTime? _fechaEmision;

        public DateTime? FechaEmision
        {
            get => _fechaEmision;
            set
            {
                // Validar si el valor es nulo
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(FechaEmision), "El valor de FechaEmision es requerido y no puede ser null.");
                }

                // Convertir a cadena para aplicar validación basada en el patrón (opcional, para mostrar más detalles si es necesario)
                string fechaStr = value.Value.ToString("dd-MM-yyyy HH:mm:ss");

                // Validar si cumple con el patrón de fecha y hora
                if (!System.Text.RegularExpressions.Regex.IsMatch(fechaStr, @"^(3[01]|[12][0-9]|0[1-9])-(1[0-2]|0[1-9])-((19|20)\d{2}) (2[0-3]|[01]?[0-9]):([0-5]?[0-9]):([0-5]?[0-9])$"))
                {
                    throw new ArgumentException($"El valor '{fechaStr}' de FechaEmision no cumple con el formato de fecha y hora válido (dd-MM-yyyy HH:mm:ss).", nameof(FechaEmision));
                }

                _fechaEmision = value;
            }
        }

        public string TerminoPago { get; set; }

        public int? IndicadorNotaCredito { get; set; }
        public int? IndicadorEnvioDiferido { get; set; }
        public int? IndicadorMontoGravado { get; set; }
        public int? TipoIngresos { get; set; }
        public bool BaseOtraMoneda { get; set; }
        public int? TipoPago { get; set; }
        public string? NumeroCuentaPago { get; set; }
        public string? BancoPago { get; set; }
        public DateTime? FechaLimitePago { get; set; }
        public DateTime? FechaDesde { get; set; } = null;
        public DateTime? FechaHasta { get; set; } = null;
        public string? TotalPaginas { get; set; }


        public required List<DetalleInterno>? Detalles
        {
            get => _detalles;
            set
            {
                // Validar si el valor es nulo
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(Detalles), "La lista Detalles es requerida y no puede ser null.");
                }

                // Validar si la lista contiene al menos un elemento
                if (value.Count == 0)
                {
                    throw new ArgumentException("La lista Detalles debe contener al menos un elemento.", nameof(Detalles));
                }

                _detalles = value;
            }
        }

        public CompradorInterno? Comprador { get; set; }
        public List<FormaPagoInterno>? FormasPago { get; set; }
        public required EmisorInterno Emisor { get; set; }
        public InformacionesAdicionalesInterno? InformacionesAdicionales { get; set; }
        public TransporteInterno? Transporte { get; set; }
        //    

        // Método auxiliar para redondear
        private decimal? Redondear(decimal? valor)
        {
            return valor == null ? null : Math.Round(valor.GetValueOrDefault() * 1.00m, 2);
        }

        public TotalInterno? Total
        {
            get
            {
                if (_total is null)
                {
                    _total = CalcularTotal();
                }
                return _total;
            }
        }



        private decimal ProcessImpuestoAdicionalOtrosImpuestos(DetalleImpuestoAdicionalInterno impuesto, ImpuestoAdicionalInterno agrupado)
        {
            decimal total = 0;

            if (impuesto.OtrosImpuestosAdicionales.HasValue)
            {
                agrupado.OtrosImpuestosAdicionales =
                    (agrupado.OtrosImpuestosAdicionales ?? 0) + impuesto.OtrosImpuestosAdicionales.Value;
                total += impuesto.OtrosImpuestosAdicionales.Value;
            }

            return total;
        }

        private TotalInterno CalcularTotal()
        {
            decimal montoNoFacturable = 0;
            decimal montoGravadoI1 = 0;
            decimal montoGravadoI2 = 0;
            decimal montoGravadoI3 = 0;
            decimal montoExento = 0;
            decimal totalITBIS1 = 0;
            decimal totalITBIS2 = 0;
            decimal totalITBIS3 = 0;
            decimal totalITBISRetenido = 0;
            decimal totalISRRetencion = 0;
            decimal montoImpuestoAdicionalImpuestoEspecificoYAdValorem = 0;
            decimal montoImpuestoAdicionalOtrosImpuestos = 0;

            decimal montoImpuestoAdicional = 0;

            var itbisRates = new Dictionary<int?, decimal>
            {
                { 0, 0.18M }, { 1, 0.18M }, { 2, 0.16M }, { 3, 0M }, { 4, 0M }
            };

            var impuestosAgrupados = new Dictionary<string, ImpuestoAdicionalInterno>();
            var hasIndicador = new bool[5]; // Para trackear ITBIS rates

            foreach (var detalle in Detalles)
            {
                if (!itbisRates.TryGetValue(detalle.IndicadorFacturacion, out var tasaITBIS))
                    continue;

                decimal montoItem = detalle.MontoItem;
                var indicador = detalle.IndicadorFacturacion.Value;

                // Cálculo de base y monto gravado
                (decimal baseGravado, decimal montoGravado) = indicador switch
                {
                    0 => (montoItem, montoItem * tasaITBIS),
                    4 => (0, 0),
                    _ => IndicadorMontoGravado == 0
                        ? (montoItem, montoItem * tasaITBIS)
                        : tasaITBIS == 0
                            ? (montoItem, 0)
                            : (montoItem / (1 + tasaITBIS), montoItem - (montoItem / (1 + tasaITBIS)))
                };



                if (IndicadorMontoGravado == 0)
                {
                    detalle.PrecioUnitarioNoGravado = detalle.PrecioUnitarioItem.GetValueOrDefault();
                }
                else
                {
                    detalle.PrecioUnitarioNoGravado = (detalle.PrecioUnitarioItem.GetValueOrDefault()
                        / (1 + tasaITBIS));
                }



                // Acumulación de montos principales
                switch (indicador)
                {
                    case 0:
                        montoNoFacturable += montoItem;
                        hasIndicador[0] = true;
                        break;
                    case 1:
                        montoGravadoI1 += baseGravado;
                        //   totalITBIS1 += montoGravado;
                        detalle.MontoITBIS = montoGravado;

                        hasIndicador[1] = true;
                        break;
                    case 2:
                        montoGravadoI2 += baseGravado;
                        //     totalITBIS2 += montoGravado;
                        detalle.MontoITBIS = montoGravado;
                        hasIndicador[2] = true;
                        break;
                    case 3:
                        montoGravadoI3 += baseGravado;
                        //    totalITBIS3 += montoGravado;
                        detalle.MontoITBIS = montoGravado;
                        hasIndicador[3] = true;
                        break;
                    case 4:
                        montoExento += montoItem;
                        hasIndicador[4] = true;
                        break;
                }





                // Impuestos adicionales
                if (detalle.ImpuestosAdicionales != null)
                {
                    foreach (var impuesto in detalle.ImpuestosAdicionales)
                    {
                        if (!impuestosAgrupados.TryGetValue(impuesto.TipoImpuesto, out var impuestoAgrupado))
                        {
                            impuestoAgrupado = new ImpuestoAdicionalInterno
                            {
                                TipoImpuesto = impuesto.TipoImpuesto,
                                TasaImpuestoAdicional = impuesto.TasaImpuestoAdicional
                            };
                            impuestosAgrupados.Add(impuesto.TipoImpuesto, impuestoAgrupado);
                        }


                        montoImpuestoAdicionalImpuestoEspecificoYAdValorem += ProcessImpuestoAdicionalImpuestoSelectivoYAdValorem(impuesto, impuestoAgrupado);
                        montoImpuestoAdicionalOtrosImpuestos += ProcessImpuestoAdicionalOtrosImpuestos(impuesto, impuestoAgrupado);

                        montoImpuestoAdicional = montoImpuestoAdicionalImpuestoEspecificoYAdValorem + montoImpuestoAdicionalOtrosImpuestos;

                    }
                }

                // Retenciones
                totalITBISRetenido += detalle.Retencion?.MontoITBISRetenido ?? 0;
                totalISRRetencion += detalle.Retencion?.MontoISRRetenido ?? 0;
            }


            if (DescuentosORecargos is not null && DescuentosORecargos.Any())
            {
                foreach (var item in DescuentosORecargos.Where(i => i.TipoValor == "$"))
                {
                    var montoAjuste = item.MontoDescuentooRecargo.GetValueOrDefault();

                    switch ((item.TipoAjuste, item.IndicadorFacturacionDescuentooRecargo))
                    {
                        case ("D", 1):
                            montoGravadoI1 -= montoAjuste;
                            break;
                        case ("D", 2):
                            montoGravadoI2 -= montoAjuste;
                            break;
                        case ("D", 3):
                            montoGravadoI3 -= montoAjuste;
                            break;

                        case ("R", 1):
                            montoGravadoI1 += montoAjuste;
                            break;
                        case ("R", 2):
                            montoGravadoI2 += montoAjuste;
                            break;
                        case ("R", 3):
                            montoGravadoI3 += montoAjuste;
                            break;

                        default:
                            // Opcional: Manejar casos inesperados o ignorarlos
                            break;
                    }
                }
            }




            totalITBIS1 = ((montoGravadoI1 + montoImpuestoAdicionalImpuestoEspecificoYAdValorem) * 0.18M);
            totalITBIS2 = montoGravadoI2 * 0.16M;
            totalITBIS3 = montoGravadoI3 * 0M;




            // Construcción del objeto final
            var total = new TotalInterno
            {
                MontoNoFacturable = montoNoFacturable == 0 ? null : Round(montoNoFacturable),
                MontoGravadoI1 = !hasIndicador[1] ? null : Round(montoGravadoI1),
                MontoGravadoI2 = !hasIndicador[2] ? null : Round(montoGravadoI2),
                MontoGravadoI3 = !hasIndicador[3] ? null : Round(montoGravadoI3),
                MontoExento = montoExento == 0 ? null : Round(montoExento),
                TotalITBIS1 = !hasIndicador[1] ? null : Round(totalITBIS1),
                TotalITBIS2 = !hasIndicador[2] ? null : Round(totalITBIS2),
                TotalITBIS3 = !hasIndicador[3] ? null : Round(totalITBIS3),
                TotalITBISRetenido = totalITBISRetenido == 0 ? null : Round(totalITBISRetenido),
                TotalISRRetencion = totalISRRetencion == 0 ? null : Round(totalISRRetencion),
                MontoImpuestoAdicional = montoImpuestoAdicional == 0 ? null : Round(montoImpuestoAdicional),
                ITBIS1 = hasIndicador[1] ? 18 : null,
                ITBIS2 = hasIndicador[2] ? 16 : null,
                ITBIS3 = hasIndicador[3] ? 0 : null,
                ValorPagar = ValorPagar != null ? Redondear(ValorPagar) : null,
                MontoPeriodo = MontoPeriodo != null ? Redondear(MontoPeriodo) : null
            };

            var montoGravadoTotal = Round(
                total.MontoGravadoI1.GetValueOrDefault() +
                total.MontoGravadoI2.GetValueOrDefault() +
                total.MontoGravadoI3.GetValueOrDefault());

            var totalItbis = montoGravadoTotal == 0 ? null : Round(
                total.TotalITBIS1.GetValueOrDefault() +
                total.TotalITBIS2.GetValueOrDefault() +
                total.TotalITBIS3.GetValueOrDefault());

            // Cálculos finales
            total.MontoGravadoTotal = montoGravadoTotal == 0 ? null : montoGravadoTotal;

            total.TotalITBIS = (montoGravadoTotal == 0)
                ? null : totalItbis;

            total.MontoTotal = Round(
               montoGravadoI1 + montoGravadoI2 + montoGravadoI3 +
               totalITBIS1 + totalITBIS2 + totalITBIS3 +
             montoExento +
              montoImpuestoAdicional);

            // Impuestos adicionales formateados
            total.ImpuestosAdicionales = impuestosAgrupados.Values
                .Select(FormatImpuestoAdicional)
                .ToList();

            return total;

        }
        // Métodos auxiliares
        private decimal ProcessImpuestoAdicionalImpuestoSelectivoYAdValorem(DetalleImpuestoAdicionalInterno impuesto, ImpuestoAdicionalInterno agrupado)
        {
            decimal total = 0;

            if (impuesto.MontoImpuestoSelectivoConsumoAdvalorem.HasValue)
            {
                agrupado.MontoImpuestoSelectivoConsumoAdvalorem =
                    (agrupado.MontoImpuestoSelectivoConsumoAdvalorem ?? 0) + impuesto.MontoImpuestoSelectivoConsumoAdvalorem.Value;
                total += impuesto.MontoImpuestoSelectivoConsumoAdvalorem.Value;
            }

            if (impuesto.MontoImpuestoSelectivoConsumoEspecifico.HasValue)
            {
                agrupado.MontoImpuestoSelectivoConsumoEspecifico =
                    (agrupado.MontoImpuestoSelectivoConsumoEspecifico ?? 0) + impuesto.MontoImpuestoSelectivoConsumoEspecifico.Value;
                total += impuesto.MontoImpuestoSelectivoConsumoEspecifico.Value;
            }

            return total;
        }

        private ImpuestoAdicionalInterno FormatImpuestoAdicional(ImpuestoAdicionalInterno impuesto)
        {
            return new ImpuestoAdicionalInterno
            {
                TipoImpuesto = impuesto.TipoImpuesto,
                TasaImpuestoAdicional = impuesto.TasaImpuestoAdicional,
                MontoImpuestoSelectivoConsumoAdvalorem = Round(impuesto.MontoImpuestoSelectivoConsumoAdvalorem),
                MontoImpuestoSelectivoConsumoEspecifico = Round(impuesto.MontoImpuestoSelectivoConsumoEspecifico),
                OtrosImpuestosAdicionales = Round(impuesto.OtrosImpuestosAdicionales)
            };
        }



        private static decimal? Round(decimal? value) => (value.HasValue) ? Math.Round(value.Value * 1.00m, 2, MidpointRounding.AwayFromZero) : null;


        public OtraMonedaInterno OtraMoneda
        {
            get
            {
                if (_otraMonedaInterno is null && TipoCambio is not null)
                {

                    decimal montoNoFacturable = 0;
                    decimal montoGravadoI1 = 0;
                    decimal montoGravadoI2 = 0;
                    decimal montoGravadoI3 = 0;
                    decimal montoExento = 0;
                    decimal totalITBIS1 = 0;
                    decimal totalITBIS2 = 0;
                    decimal totalITBIS3 = 0;
                    decimal totalITBISRetenido = 0;
                    decimal totalISRRetencion = 0;
                    decimal montoImpuestoAdicional = 0;

                    var itbisRates = new Dictionary<int?, decimal>
                    {
                        { 0, 0.18M }, { 1, 0.18M }, { 2, 0.16M }, { 3, 0M }, { 4, 0M }
                    };


                    var hasIndicador = new bool[5];

                    foreach (var detalle in Detalles)
                    {
                        if (!itbisRates.TryGetValue(detalle.IndicadorFacturacion, out var tasaITBIS))
                            continue;

                        decimal montoItem = detalle.OtraMonedaDetalle.PrecioOtraMoneda.GetValueOrDefault() * detalle.CantidadItem;

                        var indicador = detalle.IndicadorFacturacion.Value;


                        // Cálculo de base y monto gravado
                        (decimal baseGravado, decimal montoGravado) = indicador switch
                        {
                            0 => (montoItem, montoItem * tasaITBIS),
                            4 => (0, 0),
                            _ => IndicadorMontoGravado == 0
                                ? (montoItem, montoItem * tasaITBIS)
                                : tasaITBIS == 0
                                    ? (montoItem, 0)
                                    : (montoItem / (1 + tasaITBIS), montoItem - (montoItem / (1 + tasaITBIS)))
                        };

                        // Acumulación de montos principales
                        switch (indicador)
                        {
                            case 0:
                                montoNoFacturable += montoItem;
                                hasIndicador[0] = true;
                                break;
                            case 1:
                                montoGravadoI1 += baseGravado;
                                totalITBIS1 += montoGravado;
                                hasIndicador[1] = true;
                                break;
                            case 2:
                                montoGravadoI2 += baseGravado;
                                totalITBIS2 += montoGravado;
                                hasIndicador[2] = true;
                                break;
                            case 3:
                                montoGravadoI3 += baseGravado;
                                totalITBIS3 += montoGravado;
                                hasIndicador[3] = true;
                                break;
                            case 4:
                                montoExento += montoItem;
                                hasIndicador[4] = true;
                                break;
                        }


                        // Retenciones
                        totalITBISRetenido += detalle.Retencion?.MontoITBISRetenido ?? 0;
                        totalISRRetencion += detalle.Retencion?.MontoISRRetenido ?? 0;
                    }

                    // Construcción del objeto final
                    var total = new OtraMonedaInterno
                    {



                        TipoCambio = TipoCambio,
                        TipoMoneda = TipoMoneda,

                        MontoGravado1OtraMoneda = !hasIndicador[1] ? null : Round(montoGravadoI1),
                        MontoGravado2OtraMoneda = !hasIndicador[2] ? null : Round(montoGravadoI2),
                        MontoGravado3OtraMoneda = !hasIndicador[3] ? null : Round(montoGravadoI3),
                        MontoExentoOtraMoneda = montoExento == 0 ? null : Round(montoExento),
                        TotalITBIS1OtraMoneda = !hasIndicador[1] ? null : Round(totalITBIS1),
                        TotalITBIS2OtraMoneda = !hasIndicador[2] ? null : Round(totalITBIS2),
                        TotalITBIS3OtraMoneda = !hasIndicador[3] ? null : Round(totalITBIS3),

                        MontoImpuestoAdicionalOtraMoneda = montoImpuestoAdicional == 0 ? null : Round(montoImpuestoAdicional),
                    };

                    var montoGravadoTotal = Round(
                       montoGravadoI1 +
                       montoGravadoI2 +
                       montoGravadoI3);



                    // Cálculos finales
                    total.MontoGravadoTotalOtraMoneda = montoGravadoTotal == 0 ? null : montoGravadoTotal;


                    total.TotalITBISOtraMoneda = montoGravadoTotal == 0 ? null :
                        Math.Round((totalITBIS1 + totalITBIS2 + totalITBIS3) * 1.00m, 2);

                    total.MontoTotalOtraMoneda = Round(
                       montoGravadoI1 + montoGravadoI2 + montoGravadoI3 +
                       totalITBIS1 + totalITBIS2 + totalITBIS3 +
                     montoExento +
                       montoNoFacturable +
                      montoImpuestoAdicional);

                    _otraMonedaInterno = total;
                }
                return _otraMonedaInterno;
            }
            set
            {
                _otraMonedaInterno = value;
            }
        }

        public OtraMonedaInterno? OtraMoneda2
        {
            get
            {
                if (_otraMonedaInterno is null)
                {

                    if (_otraMonedaInterno is null && UsaOtraMoneda)
                    {
                        var otraMoneda = new OtraMonedaInterno();

                        var itbisRates = new Dictionary<int?, decimal>
                    {
                        { 1, 0.18M },
                        { 2, 0.16M },
                        { 3, 0M },
                        { 4, 0M}
                    };

                        foreach (var detalle in Detalles)
                        {
                            if (itbisRates.ContainsKey(detalle.IndicadorFacturacion))
                            {
                                decimal montoItemOtraMoneda = detalle.OtraMonedaDetalle.MontoItemOtraMoneda.GetValueOrDefault();
                                decimal tasaITBIS = itbisRates[detalle.IndicadorFacturacion];

                                decimal montoGravado = montoItemOtraMoneda * tasaITBIS;

                                switch (detalle.IndicadorFacturacion)
                                {
                                    case 1:
                                        otraMoneda.MontoGravado1OtraMoneda = otraMoneda.MontoGravado1OtraMoneda.GetValueOrDefault() + montoItemOtraMoneda;
                                        otraMoneda.TotalITBIS1OtraMoneda = otraMoneda.TotalITBIS1OtraMoneda.GetValueOrDefault() + montoGravado;
                                        break;

                                    case 2:
                                        otraMoneda.MontoGravado2OtraMoneda = otraMoneda.MontoGravado2OtraMoneda.GetValueOrDefault() + montoItemOtraMoneda;
                                        otraMoneda.TotalITBIS2OtraMoneda = otraMoneda.TotalITBIS2OtraMoneda.GetValueOrDefault() + montoGravado;
                                        break;

                                    case 3:
                                        otraMoneda.MontoGravado3OtraMoneda = otraMoneda.MontoGravado3OtraMoneda.GetValueOrDefault() + montoItemOtraMoneda;
                                        otraMoneda.TotalITBIS3OtraMoneda = otraMoneda.TotalITBIS3OtraMoneda.GetValueOrDefault() + montoGravado;
                                        break;
                                    case 4:
                                        otraMoneda.MontoExentoOtraMoneda = otraMoneda.MontoExentoOtraMoneda.GetValueOrDefault() + montoItemOtraMoneda;
                                        break;
                                }

                                if (detalle.IndicadorFacturacion != 4 &&
                                    detalle.IndicadorFacturacion != 0)
                                {
                                    otraMoneda.TotalITBISOtraMoneda =
                                      otraMoneda.TotalITBIS1OtraMoneda.GetValueOrDefault() +
                                      otraMoneda.TotalITBIS2OtraMoneda.GetValueOrDefault()
                                      + otraMoneda.TotalITBIS3OtraMoneda.GetValueOrDefault();
                                    otraMoneda.MontoGravadoTotalOtraMoneda =
                                    otraMoneda.MontoGravado1OtraMoneda.GetValueOrDefault() +
                                    otraMoneda.MontoGravado2OtraMoneda.GetValueOrDefault()
                                    + otraMoneda.MontoGravado3OtraMoneda.GetValueOrDefault();

                                    otraMoneda.MontoTotalOtraMoneda = otraMoneda.MontoGravadoTotalOtraMoneda.GetValueOrDefault() +
                                        otraMoneda.TotalITBISOtraMoneda.GetValueOrDefault();
                                }
                                else
                                {

                                    otraMoneda.MontoTotalOtraMoneda = otraMoneda.MontoTotalOtraMoneda.GetValueOrDefault() +
                                    montoItemOtraMoneda;
                                }
                            }
                            otraMoneda.MontoGravado1OtraMoneda = otraMoneda.MontoGravado1OtraMoneda == null ? null : Math.Round(otraMoneda.MontoGravado1OtraMoneda.GetValueOrDefault(), 2);
                            otraMoneda.MontoGravado2OtraMoneda = otraMoneda.MontoGravado2OtraMoneda == null ? null : Math.Round(otraMoneda.MontoGravado2OtraMoneda.GetValueOrDefault(), 2);
                            otraMoneda.MontoGravado3OtraMoneda = otraMoneda.MontoGravado3OtraMoneda == null ? null : Math.Round(otraMoneda.MontoGravado3OtraMoneda.GetValueOrDefault(), 2);
                            otraMoneda.MontoExentoOtraMoneda = otraMoneda.MontoExentoOtraMoneda == null ? null : Math.Round(otraMoneda.MontoExentoOtraMoneda.GetValueOrDefault(), 2);
                            otraMoneda.MontoTotalOtraMoneda = otraMoneda.MontoTotalOtraMoneda == null ? null : Math.Round(otraMoneda.MontoTotalOtraMoneda.GetValueOrDefault(), 2);
                            otraMoneda.TotalITBIS1OtraMoneda = otraMoneda.TotalITBIS1OtraMoneda == null ? null : Math.Round(otraMoneda.TotalITBIS1OtraMoneda.GetValueOrDefault(), 2);
                            otraMoneda.TotalITBIS2OtraMoneda = otraMoneda.TotalITBIS2OtraMoneda == null ? null : Math.Round(otraMoneda.TotalITBIS2OtraMoneda.GetValueOrDefault(), 2);
                            otraMoneda.TotalITBIS3OtraMoneda = otraMoneda.TotalITBIS3OtraMoneda == null ? null : Math.Round(otraMoneda.TotalITBIS3OtraMoneda.GetValueOrDefault(), 2);
                            otraMoneda.MontoGravadoTotalOtraMoneda = otraMoneda.MontoGravadoTotalOtraMoneda == null ? null : Math.Round(otraMoneda.MontoGravadoTotalOtraMoneda.GetValueOrDefault(), 2);
                            otraMoneda.TotalITBISOtraMoneda = otraMoneda.TotalITBISOtraMoneda == null ? null : Math.Round(otraMoneda.TotalITBISOtraMoneda.GetValueOrDefault(), 2);

                            otraMoneda.TipoMoneda = TipoMoneda;
                            otraMoneda.TipoCambio = TipoCambio;

                            _otraMonedaInterno = otraMoneda;
                        }
                    }

                }

                return _otraMonedaInterno;
            }
            set
            {
                _otraMonedaInterno = value;
            }
        }
        public List<PaginaInterno>? Paginas { get; set; }
        public InformacionReferenciaInterno? InformacionReferencia { get; set; }
        public List<SubTotalInterno>? SubTotales { get; set; }
        public List<DescuentoORecargoInterno>? DescuentosORecargos { get; set; }
    }
}