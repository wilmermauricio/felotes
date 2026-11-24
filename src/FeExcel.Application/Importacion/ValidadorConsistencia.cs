using FeExcel.Domain.Errores;

namespace FeExcel.Application.Importacion;

/// <summary>
/// Verifica que todas las filas de un comprobante repitan los mismos valores de encabezado.
/// Una diferencia es una inconsistencia en el archivo y se reporta por campo.
/// </summary>
public sealed class ValidadorConsistencia
{
    public IReadOnlyList<ErrorImportacion> Validar(GrupoComprobante grupo)
    {
        if (grupo.Filas.Count < 2)
            return [];

        var primera = grupo.Filas[0];
        var errores = new List<ErrorImportacion>();

        foreach (var campo in CamposEncabezado.Nombres)
        {
            var valorEsperado = CamposEncabezado.Valor(primera.Encabezado, campo);

            var filasDistintas = grupo.Filas
                .Skip(1)
                .Where(f => !Equals(CamposEncabezado.Valor(f.Encabezado, campo), valorEsperado))
                .Select(f => f.NumeroFila)
                .ToList();

            if (filasDistintas.Count == 0)
                continue;

            var filas = UnirNumeros([primera.NumeroFila, .. filasDistintas]);
            errores.Add(new ErrorImportacion(null, campo,
                $"Inconsistencia en '{campo}': las filas {filas} tienen valores distintos"));
        }

        return errores;
    }

    /// <summary>[2, 3, 5] → "2, 3 y 5".</summary>
    private static string UnirNumeros(IReadOnlyList<int> numeros) =>
        numeros.Count == 1
            ? numeros[0].ToString()
            : $"{string.Join(", ", numeros.Take(numeros.Count - 1))} y {numeros[^1]}";
}
