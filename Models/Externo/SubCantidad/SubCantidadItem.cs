using DGII.FE.XML.Models.Interno;

namespace DGII.FE.XML.Models.Externo
{
    public class SubcantidadItem
    {
        public decimal? Subcantidad { get; set; }
        public string? CodigoSubcantidad { get; set; }

        public static List<SubcantidadItemInterno> ToSubCantidadInterno(List<SubcantidadItem> subcantidadItems)
        {
            return subcantidadItems?.Select(x => new SubcantidadItemInterno
            {
                CodigoSubcantidad = x?.CodigoSubcantidad,
                Subcantidad = x?.Subcantidad
            }).ToList();
        }
    }
}
