using System.Reflection;
using FeExcel.Application.Plantilla;

namespace FeExcel.Application.Importacion;

/// <summary>
/// Acceso por nombre de columna a los campos de <see cref="EncabezadoFila"/>.
/// Las propiedades se llaman igual que las columnas del Excel, así que agregar una columna
/// de encabezado no requiere tocar los validadores.
/// </summary>
internal static class CamposEncabezado
{
    private static readonly IReadOnlyDictionary<string, PropertyInfo> Propiedades =
        typeof(EncabezadoFila)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToDictionary(p => p.Name);

    public static IEnumerable<string> Nombres => Propiedades.Keys;

    public static object? Valor(EncabezadoFila encabezado, string nombreColumna) =>
        Propiedades.TryGetValue(nombreColumna, out var propiedad)
            ? propiedad.GetValue(encabezado)
            : throw new ArgumentException($"'{nombreColumna}' no es un campo de encabezado.", nameof(nombreColumna));

    public static bool TieneValor(EncabezadoFila encabezado, string nombreColumna) =>
        Valor(encabezado, nombreColumna) is not null;
}
