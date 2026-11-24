using System.Globalization;
using ClosedXML.Excel;
using FeExcel.Application.Plantilla;
using FeExcel.Domain.Errores;

namespace FeExcel.Infrastructure.Excel;

/// <summary>
/// Lee las celdas de una fila y las convierte al tipo esperado.
/// Si un valor no se puede convertir, registra el error (con fila y columna) y devuelve <c>null</c>.
/// Una columna que no existe en el archivo se lee siempre como vacía.
/// </summary>
internal sealed class LectorCeldas(IXLRow fila, IReadOnlyDictionary<string, int> columnas, ICollection<ErrorImportacion> errores)
{
    private static readonly string[] FormatosFecha = ["dd-MM-yyyy", "dd/MM/yyyy", "yyyy-MM-dd", "d-M-yyyy", "d/M/yyyy"];
    private static readonly CultureInfo Invariante = CultureInfo.InvariantCulture;

    public int NumeroFila => fila.RowNumber();

    public bool EstaVacia() => columnas.Values.All(numero => fila.Cell(numero).Value.IsBlank || EsTextoVacio(fila.Cell(numero)));

    public string? Texto(ColumnaPlantilla columna)
    {
        var valor = Celda(columna)?.Value;

        if (valor is not { } v || v.IsBlank)
            return null;

        // Un RNC escrito como número no debe salir como "1.31E+08".
        var texto = v.IsNumber ? v.GetNumber().ToString("0.##########", Invariante) : v.ToString(Invariante);
        return string.IsNullOrWhiteSpace(texto) ? null : texto.Trim();
    }

    public int? Entero(ColumnaPlantilla columna) => Convertir<int>(columna, "un número entero válido", valor =>
    {
        if (valor.IsNumber && valor.GetNumber() is var numero && numero == Math.Floor(numero))
            return (int)numero;

        return int.TryParse(TextoDe(valor), NumberStyles.Integer, Invariante, out var entero) ? entero : null;
    });

    public decimal? Decimal(ColumnaPlantilla columna) => Convertir<decimal>(columna, "un número válido", valor =>
    {
        if (valor.IsNumber)
            return (decimal)valor.GetNumber();

        return decimal.TryParse(TextoDe(valor), NumberStyles.Number, Invariante, out var numero) ? numero : null;
    });

    public DateTime? Fecha(ColumnaPlantilla columna) => Convertir<DateTime>(columna, "una fecha válida (use dd-MM-yyyy)", valor =>
    {
        if (valor.IsDateTime)
            return valor.GetDateTime();

        if (valor.IsNumber)
            return DateTime.FromOADate(valor.GetNumber());

        return DateTime.TryParseExact(TextoDe(valor), FormatosFecha, Invariante, DateTimeStyles.None, out var fecha) ? fecha : null;
    });

    public bool? Booleano(ColumnaPlantilla columna) => Convertir<bool>(columna, "un valor Si/No válido", valor =>
    {
        if (valor.IsBoolean)
            return valor.GetBoolean();

        return TextoDe(valor).ToUpperInvariant() switch
        {
            "1" or "SI" or "SÍ" or "S" or "TRUE" or "VERDADERO" => true,
            "0" or "NO" or "N" or "FALSE" or "FALSO" => false,
            _ => (bool?)null
        };
    });

    /// <summary>Aplica la conversión; si la celda tiene valor pero no se pudo convertir, registra el error.</summary>
    private T? Convertir<T>(ColumnaPlantilla columna, string descripcionEsperada, Func<XLCellValue, T?> conversion)
        where T : struct
    {
        var celda = Celda(columna);
        if (celda is null || celda.Value.IsBlank || EsTextoVacio(celda))
            return null;

        T? resultado;
        try
        {
            resultado = conversion(celda.Value);
        }
        catch (Exception ex) when (ex is ArgumentException or OverflowException or InvalidCastException)
        {
            resultado = null;
        }

        if (resultado is null)
        {
            var textoOriginal = celda.GetFormattedString();
            errores.Add(ErrorImportacion.EnCelda(NumeroFila, columna.Nombre, $"'{textoOriginal}' no es {descripcionEsperada}"));
        }

        return resultado;
    }

    private IXLCell? Celda(ColumnaPlantilla columna) =>
        columnas.TryGetValue(columna.Nombre, out var numero) ? fila.Cell(numero) : null;

    private static string TextoDe(XLCellValue valor) => valor.ToString(Invariante).Trim();

    private static bool EsTextoVacio(IXLCell celda) => celda.Value.IsText && string.IsNullOrWhiteSpace(celda.Value.GetText());
}
