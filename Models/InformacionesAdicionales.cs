using DGII.FE.XML.Models.Interno;

namespace DGII.FE.XML.Models
{
    public class InformacionesAdicionales
    {
        public DateTime? FechaEmbarque { get; set; }
        public string? NumeroEmbarque { get; set; }
        public string? NumeroContenedor { get; set; }
        public int NumeroReferencia { get; set; }
        public string? NombrePuertoEmbarque { get; set; }
        public string? CondicionesEntrega { get; set; }
        public decimal? TotalFob { get; set; }
        public decimal? Seguro { get; set; }
        public decimal? Flete { get; set; }
        public decimal? OtrosGastos { get; set; }
        public decimal? TotalCif { get; set; }
        public string? RegimenAduanero { get; set; }
        public string? NombrePuertoSalida { get; set; }
        public decimal? PesoBruto { get; set; }
        public decimal? PesoNeto { get; set; }
        public int? UnidadPesoBruto { get; set; }
        public int? UnidadPesoNeto { get; set; }
        public decimal? CantidadBulto { get; set; }
        public int? UnidadBulto { get; set; }
        public decimal? VolumenBulto { get; set; }
        public int? UnidadVolumen { get; set; }

        internal static InformacionesAdicionalesInterno
            ToInformacionesAdicionalesInterno(InformacionesAdicionales informacionesAdicionales)
        {
            if (informacionesAdicionales == null) return null;

            return new InformacionesAdicionalesInterno
            {

                CantidadBulto = informacionesAdicionales?.CantidadBulto != null ? Math.Round(informacionesAdicionales.CantidadBulto.Value * 1.00m, 2) : null,
                CondicionesEntrega = informacionesAdicionales?.CondicionesEntrega,
                FechaEmbarque = informacionesAdicionales?.FechaEmbarque,
                Flete = informacionesAdicionales?.Flete != null ? Math.Round(informacionesAdicionales.Flete.Value * 1.00m, 2) : null,
                NombrePuertoEmbarque = informacionesAdicionales?.NombrePuertoEmbarque,
                NombrePuertoSalida = informacionesAdicionales?.NombrePuertoSalida,
                NumeroContenedor = informacionesAdicionales?.NumeroContenedor,
                NumeroEmbarque = informacionesAdicionales?.NumeroEmbarque,
                NumeroReferencia = informacionesAdicionales?.NumeroReferencia,
                OtrosGastos = informacionesAdicionales?.OtrosGastos != null ? Math.Round(informacionesAdicionales.OtrosGastos.Value * 1.00m, 2) : null,
                PesoBruto = informacionesAdicionales?.PesoBruto != null ? Math.Round(informacionesAdicionales.PesoBruto.Value * 1.00m, 2) : null,
                PesoNeto = informacionesAdicionales?.PesoNeto != null ? Math.Round(informacionesAdicionales.PesoNeto.Value * 1.00m, 2) : null,
                RegimenAduanero = informacionesAdicionales?.RegimenAduanero,
                Seguro = informacionesAdicionales?.Seguro != null ? Math.Round(informacionesAdicionales.Seguro.Value * 1.00m, 2) : null,
                TotalCif = informacionesAdicionales?.TotalCif != null ? Math.Round(informacionesAdicionales.TotalCif.Value * 1.00m, 2) : null,
                TotalFob = informacionesAdicionales?.TotalFob != null ? Math.Round(informacionesAdicionales.TotalFob.Value * 1.00m, 2) : null,
                UnidadBulto = informacionesAdicionales?.UnidadBulto,
                UnidadPesoBruto = informacionesAdicionales?.UnidadPesoBruto,
                UnidadPesoNeto = informacionesAdicionales?.UnidadPesoNeto,
                UnidadVolumen = informacionesAdicionales?.UnidadVolumen,
                VolumenBulto = informacionesAdicionales?.VolumenBulto != null ? Math.Round(informacionesAdicionales.VolumenBulto.Value * 1.00m, 2) : null
            };

        }
    }
}
