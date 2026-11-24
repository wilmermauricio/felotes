using ClosedXML.Excel;
using FeExcel.Application.Abstracciones;
using FeExcel.Application.Plantilla;

namespace FeExcel.Infrastructure.Excel;

/// <summary>
/// Genera la plantilla vacía: una hoja con los encabezados en negrita, la fila congelada,
/// una nota de ayuda en cada encabezado y formato de fecha o texto en las columnas que lo requieren.
/// </summary>
public sealed class GeneradorPlantillaClosedXml : IGeneradorPlantilla
{
    private const string NombreHoja = "Comprobantes";
    private const string FormatoFecha = "dd-MM-yyyy";
    private const string FormatoTexto = "@";
    private static readonly XLColor ColorObligatoria = XLColor.FromHtml("#DDEBF7");

    public byte[] Generar()
    {
        using var libro = new XLWorkbook();
        var hoja = libro.Worksheets.Add(NombreHoja);

        for (var indice = 0; indice < ColumnasPlantilla.Todas.Count; indice++)
        {
            var columna = ColumnasPlantilla.Todas[indice];
            var numeroColumna = indice + 1;
            var encabezado = hoja.Cell(1, numeroColumna);

            encabezado.Value = columna.Nombre;
            encabezado.Style.Font.Bold = true;
            encabezado.CreateComment().AddText(columna.Ayuda);

            if (columna.ObligatoriaEnArchivo)
                encabezado.Style.Fill.BackgroundColor = ColorObligatoria;

            AplicarFormato(hoja.Column(numeroColumna), columna.Tipo);
        }

        hoja.SheetView.FreezeRows(1);
        hoja.Columns().AdjustToContents();

        using var salida = new MemoryStream();
        libro.SaveAs(salida);
        return salida.ToArray();
    }

    /// <summary>
    /// Las columnas de texto usan formato "@" para que Excel no convierta un RNC o un ENCF en número.
    /// La fila de encabezados conserva su propio formato.
    /// </summary>
    private static void AplicarFormato(IXLColumn columna, TipoDato tipo)
    {
        var formato = tipo switch
        {
            TipoDato.Fecha => FormatoFecha,
            TipoDato.Texto => FormatoTexto,
            _ => null
        };

        if (formato is not null)
            columna.Style.NumberFormat.Format = formato;
    }
}
