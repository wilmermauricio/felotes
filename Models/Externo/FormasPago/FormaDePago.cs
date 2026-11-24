using DGII.FE.XML.Models.Interno;

namespace DGII.FE.XML.Models.Externo.FormasPago
{
    public class FormaDePago
    {
        private int? _formaPago;

        public int? FormaPago
        {
            get => _formaPago;
            set
            {
                if (value != null && (value < 1 || value > 8))
                {
                    throw new ArgumentOutOfRangeException(nameof(FormasPago.FormaDePago),
                        $"El valor {value} no es válido para FormaPago. Valores válidos: " +
                        "1 (Efectivo), 2 (Cheque/Transferencia/Depósito), 3 (Tarjeta de Débito/Crédito), " +
                        "4 (Venta a Crédito), 5 (Bonos o Certificados de regalo), 6 (Permuta), " +
                        "7 (Nota de crédito), 8 (Otras Formas de pago).");
                }

                _formaPago = value;
            }
        }
        public decimal? MontoPago { get; set; }

        public static List<FormaPagoInterno> ToFormasPagoInterno(List<FormaDePago> formasPago)
        {
            return formasPago?.Select(x => new FormaPagoInterno
            {
                FormaPago = x.FormaPago,
                MontoPago = x.MontoPago
            }).ToList();
        }
    }
}
