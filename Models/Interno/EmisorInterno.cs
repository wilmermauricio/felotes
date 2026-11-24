namespace DGII.FE.XML.Models.Interno
{
    public class EmisorInterno
    {
        private string? _rnc;
        private string? _razonSocial;
        public string RNC
        {
            get => _rnc!;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("El valor de RNC es requerido y no puede ser null, vacío o solo espacios en blanco.", nameof(RNC));
                }

                if (!System.Text.RegularExpressions.Regex.IsMatch(value, "^[0-9]{9}$|^[0-9]{11}$"))
                {
                    throw new ArgumentException("El valor de RNC debe ser una cadena de 9 o 11 dígitos.", nameof(RNC));
                }

                _rnc = value;
            }
        }
        public string RazonSocial
        {
            get => _razonSocial!;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("El valor de RazonSocial es requerido y no puede ser null, vacío o solo espacios en blanco.", nameof(RazonSocial));
                }

                if (value.Length < 1 || value.Length > 150)
                {
                    throw new ArgumentException("El valor de RazonSocial debe tener entre 1 y 150 caracteres.", nameof(RazonSocial));
                }

                _razonSocial = value;
            }
        }

        public string? NombreComercial { get; set; }
        public string? Sucursal { get; set; }
        public string? Direccion { get; set; }
        public string? Municipio { get; set; }
        public string? Provincia { get; set; }
        public string? Correo { get; set; }
        public string? WebSite { get; set; }
        public string? ActividadEconomica { get; set; }
        public string? CodigoVendedor { get; set; }
        public string? NumeroFacturaInterna { get; set; }
        public long? NumeroPedidoInterno { get; set; }
        public string? ZonaVenta { get; set; }
        public string? RutaVenta { get; set; }
        public string? InformacionAdicionalEmisor { get; set; }
        public List<string> Telefonos { get; set; }
    }
}


