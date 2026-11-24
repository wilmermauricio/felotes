using DGII.FE.XML.Models.Interno;

namespace DGII.FE.XML.Models.Externo
{
    public class Emisor
    {
        public required string RNCEmisor { get; set; }
        public required string RazonSocialEmisor { get; set; }
        public string? NombreComercial { get; set; }
        public string? Sucursal { get; set; }
        public string? DireccionEmisor { get; set; }
        public string Municipio { get; set; }
        public string Provincia { get; set; }
        public string? CorreoEmisor { get; set; }
        public string? WebSite { get; set; }
        public string? ActividadEconomica { get; set; }
        public string? CodigoVendedor { get; set; }
        public string? NumeroFacturaInterna { get; set; }
        public long? NumeroPedidoInterno { get; set; }
        public string? ZonaVenta { get; set; }
        public string? RutaVenta { get; set; }
        public string? InformacionAdicionalEmisor { get; set; }
        public List<string> Telefonos { get; set; }
        public static EmisorInterno ToEmisorInterno(Emisor emisor)
        {
            var emisorInterno = new EmisorInterno
            {
                Direccion = emisor.DireccionEmisor,
                Correo = emisor.CorreoEmisor,
                ActividadEconomica = emisor.ActividadEconomica,
                CodigoVendedor = emisor.CodigoVendedor,
                InformacionAdicionalEmisor = emisor.InformacionAdicionalEmisor,
                Municipio = emisor.Municipio,
                NombreComercial = emisor.NombreComercial,
                NumeroFacturaInterna = emisor.NumeroFacturaInterna,
                Sucursal = emisor.Sucursal,
                RazonSocial = emisor.RazonSocialEmisor,
                WebSite = emisor.WebSite,
                NumeroPedidoInterno = emisor.NumeroPedidoInterno,
                Provincia = emisor.Provincia,
                RNC = emisor.RNCEmisor,
                RutaVenta = emisor.RutaVenta,
                Telefonos = emisor.Telefonos,
                ZonaVenta = emisor.ZonaVenta
            };
            return emisorInterno;
        }
    }
}




