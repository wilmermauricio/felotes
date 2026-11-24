using System.Globalization;
using System.Reflection;
using System.Text;
using FeExcel.Application.Plantilla;

namespace FeExcel.Web.Models;

/// <summary>
/// Arma el texto en el que busca el buscador de la vista previa. Incluye campos que la tabla
/// no muestra (NCF modificado, moneda, etc.). El navegador normaliza lo que escribe el usuario
/// igual que <see cref="Normalizar"/> (ver <c>site.js</c>).
/// </summary>
public static class Busqueda
{
    private const string FormatoFecha = "dd-MM-yyyy";

    /// <summary>Todas las columnas de encabezado: una columna nueva entra sola en la búsqueda.</summary>
    private static readonly PropertyInfo[] CamposEncabezado = typeof(EncabezadoFila).GetProperties();

    public static string Texto(
        string empresa,
        string encf,
        string tipo,
        EncabezadoFila encabezado,
        decimal? totalItbis,
        decimal? montoTotal,
        IEnumerable<DetalleVistaPrevia> detalles,
        IEnumerable<string> errores)
    {
        IEnumerable<string?> partes =
        [
            empresa, encf, tipo, Formatear(totalItbis), Formatear(montoTotal),
            .. CamposEncabezado.Select(p => Formatear(p.GetValue(encabezado))),
            .. detalles.SelectMany(d => new[]
            {
                d.Nombre, Formatear(d.Cantidad), Formatear(d.PrecioUnitario), Formatear(d.MontoItem)
            }),
            .. errores
        ];

        return Normalizar(string.Join(' ', partes.Where(p => !string.IsNullOrWhiteSpace(p))));
    }

    /// <summary>Minúsculas y sin acentos: "Crédito" → "credito".</summary>
    public static string Normalizar(string texto)
    {
        var descompuesto = texto.ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var sinAcentos = descompuesto.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);
        return new string(sinAcentos.ToArray()).Normalize(NormalizationForm.FormC);
    }

    private static string? Formatear(object? valor) => valor switch
    {
        null => null,
        DateTime fecha => fecha.ToString(FormatoFecha, CultureInfo.InvariantCulture),
        decimal numero => $"{numero.ToString("0.##", CultureInfo.InvariantCulture)} {numero.ToString("N2", CultureInfo.InvariantCulture)}",
        bool valorLogico => valorLogico ? "si" : "no",
        _ => Convert.ToString(valor, CultureInfo.InvariantCulture)
    };
}
