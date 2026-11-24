using System.Globalization;

namespace DGII.FE.XML.Models.Interno
{
    public class DetalleInterno
    {
        private int? _indicadorFacturacion;
        private string? _nombreItem;
        private decimal _montoItem;
        private int? _indicadorBienoServicio;
        private decimal _cantidadItem;
        private decimal? _precioUnitarioItem;
        public int NumeroLinea { get; set; }
        public int? IndicadorFacturacion
        {
            get => _indicadorFacturacion;
            set
            {
                if (value == null || value < 0 || value > 4)
                {
                    throw new ArgumentException("El valor de IndicadorFacturacion debe estar entre 1 y 4.", nameof(IndicadorFacturacion));
                }

                _indicadorFacturacion = value;
            }
        }

        public decimal MontoITBIS { get; set; }
        public decimal PrecioUnitarioNoGravado { get; set; }
        public required string NombreItem
        {
            get => _nombreItem!;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("El valor de NombreItem es requerido y no puede ser null, vacío o solo espacios en blanco.", nameof(NombreItem));
                }

                if (value.Length < 1 || value.Length > 80)
                {
                    throw new ArgumentException("El valor de NombreItem debe tener entre 1 y 80 caracteres.", nameof(NombreItem));
                }

                _nombreItem = value;
            }
        }
        public int IndicadorBienoServicio
        {
            get => _indicadorBienoServicio ?? throw new InvalidOperationException("El valor de IndicadorBienoServicio es requerido y no puede ser null.");
            set
            {
                if (value != 1 && value != 2)
                {
                    throw new ArgumentException("El valor de IndicadorBienoServicio debe ser 1 (Bien) o 2 (Servicio).", nameof(IndicadorBienoServicio));
                }

                _indicadorBienoServicio = value;
            }
        }
        public string? DescripcionItem { get; set; }
        public required decimal CantidadItem
        {
            get => _cantidadItem;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentException("El valor de CantidadItem debe ser mayor que cero.", nameof(CantidadItem));
                }

                string cantidadStr = value.ToString("F2", CultureInfo.InvariantCulture);

                if (!System.Text.RegularExpressions.Regex.IsMatch(cantidadStr, @"^[0-9]{1,16}(\.[0-9]{1,2})?$"))
                {
                    throw new ArgumentException("El valor de CantidadItem debe tener hasta 16 dígitos enteros y un máximo de 2 decimales.", nameof(CantidadItem));
                }
                _cantidadItem = value;
            }
        }
        public int? UnidadMedida { get; set; }
        public decimal? CantidadReferencia { get; set; }
        public string? UnidadReferencia { get; set; }
        public decimal? GradosAlcohol { get; set; }
        public decimal? PrecioUnitarioReferencia { get; set; }
        public DateTime? FechaElaboracion { get; set; }
        public DateTime? FechaVencimientoItem { get; set; }

        public decimal? PrecioUnitarioItem { get; set; }


        public decimal? DescuentoMonto { get; set; }

        public decimal? RecargoMonto { get; set; }
        public required decimal MontoItem
        {
            get => _montoItem;
            set
            {

                if (value < 0)
                {
                    throw new ArgumentException("El valor de MontoItem debe ser mayor o igual a cero.", nameof(MontoItem));
                }

                string montoStr = value.ToString("0.##");

                if (!System.Text.RegularExpressions.Regex.IsMatch(montoStr, @"^\d{1,16}(\.\d{1,2})?$"))
                {
                    throw new ArgumentException("El valor de MontoItem debe tener hasta 16 dígitos enteros y un máximo de 2 decimales.", nameof(MontoItem));
                }

                _montoItem = value;
            }
        }
        public List<CodigosItemInterno>? CodigosItems { get; set; }
        public List<SubcantidadItemInterno>? SubcantidadItems { get; set; }
        public List<SubDescuentoInterno>? SubDescuentos { get; set; }
        public List<DetalleImpuestoAdicionalInterno>? ImpuestosAdicionales { get; set; }
        public List<SubRecargoInterno> SubRecargos { get; set; }
        public OtraMonedaDetalleInterno? OtraMonedaDetalle { get; set; }
        public RetencionInterno Retencion { get; set; }

        /*
         
               public decimal? GradosAlcohol { get; set; }
           public decimal? PrecioUnitarioReferencia { get; set; }
         */
    }
}
