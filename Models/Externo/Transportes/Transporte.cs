using DGII.FE.XML.Models.Interno;

namespace DGII.FE.XML.Models.Externo.Transportes
{
    public class Transporte
    {
        public string? ViaTransporte { get; set; }
        public string? PaisOrigen { get; set; }
        public string? DireccionDestino { get; set; }
        public string? PaisDestino { get; set; }
        public string? RNCIdentificacionCompaniaTransportista { get; set; }
        public string? NombreCompaniaTransportista { get; set; }
        public string? NumeroViaje { get; set; }
        public string? Conductor { get; set; }
        public int? DocumentoTransporte { get; set; }
        public string? Ficha { get; set; }
        public string? Placa { get; set; }
        public string? RutaTransporte { get; set; }
        public string? ZonaTransporte { get; set; }
        public string? NumeroAlbaran { get; set; }

        public static TransporteInterno ToTransporteInterno(Transporte transporte)
        {
            if (transporte == null)
            {
                return null;
            }

            var transporteInterno = new TransporteInterno
            {
                DireccionDestino = transporte?.DireccionDestino,
                DocumentoTransporte = transporte?.DocumentoTransporte,
                PaisDestino = transporte?.PaisDestino,
                Conductor = transporte?.Conductor,
                Ficha = transporte?.Ficha,
                NombreCompaniaTransportista = transporte?.NombreCompaniaTransportista,
                NumeroAlbaran = transporte?.NumeroAlbaran,
                NumeroViaje = transporte?.NumeroViaje,
                PaisOrigen = transporte?.PaisOrigen,
                Placa = transporte?.Placa,
                RNCIdentificacionCompaniaTransportista = transporte?.RNCIdentificacionCompaniaTransportista,
                RutaTransporte = transporte?.RutaTransporte,
                ViaTransporte = transporte?.ViaTransporte,
                ZonaTransporte = transporte?.ZonaTransporte
            };

            return transporteInterno;
        }
    }
}
