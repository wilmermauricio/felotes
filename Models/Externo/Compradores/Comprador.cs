using DGII.FE.XML.Models.Interno;

namespace DGII.FE.XML.Models.Externo.Compradores
{
    public abstract class Comprador
    {
        protected Comprador() { }
        public string? RazonSocialComprador { get; set; }
        public string? ContactoComprador { get; set; }
        public string? CorreoComprador { get; set; }
        public string? DireccionComprador { get; set; }
        public string? MunicipioComprador { get; set; }
        public string? ProvinciaComprador { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public string? ContactoEntrega { get; set; }
        public string? DireccionEntrega { get; set; }
        public string? TelefonoAdicional { get; set; }
        public DateTime? FechaOrdenCompra { get; set; }
        public string? NumeroOrdenCompra { get; set; }
        public string? CodigoInternoComprador { get; set; }
        public string? ResponsablePago { get; set; }
        public string? InformacionAdicionalComprador { get; set; }
        public static CompradorInterno ToCompradorInterno<T>(T comprador) where T : Comprador
        {

            if (comprador is null)
            {
                return null;
            }

            var compradorInterno = new CompradorInterno
            {
                DireccionComprador = comprador.DireccionComprador,
                DireccionEntrega = comprador.DireccionEntrega,
                CodigoInternoComprador = comprador.CodigoInternoComprador,
                ContactoEntrega = comprador.ContactoEntrega,
                ContactoComprador = comprador.ContactoComprador,
                CorreoComprador = comprador.CorreoComprador,
                FechaEntrega = comprador.FechaEntrega,
                FechaOrdenCompra = comprador.FechaOrdenCompra,
                InformacionAdicionalComprador = comprador.InformacionAdicionalComprador,
                MunicipioComprador = comprador.MunicipioComprador,
                NumeroOrdenCompra = comprador.NumeroOrdenCompra,
                ProvinciaComprador = comprador.ProvinciaComprador,
                RazonSocialComprador = comprador.RazonSocialComprador,
                ResponsablePago = comprador.ResponsablePago,
                TelefonoAdicional = comprador.TelefonoAdicional,
            };

            switch (comprador)
            {
                case CompradorNacional compradorNacional:
                    compradorInterno.RNCComprador = compradorNacional.RNCComprador;
                    break;
                case CompradorExtranjero compradorExtranjero:
                    compradorInterno.IdentificadorExtranjero = compradorExtranjero.IdentificadorExtranjero;
                    break;
            }
            return compradorInterno;
        }
    }
}
