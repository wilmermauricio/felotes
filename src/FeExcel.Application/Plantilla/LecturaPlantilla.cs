using FeExcel.Domain.Errores;

namespace FeExcel.Application.Plantilla;

/// <summary>
/// Resultado de leer el Excel.
/// <see cref="ErroresArchivo"/> impiden procesar el archivo (por ejemplo, falta una columna obligatoria);
/// <see cref="ErroresCeldas"/> son valores con formato inválido, asociados a su fila.
/// </summary>
public sealed record LecturaPlantilla(
    IReadOnlyList<FilaComprobante> Filas,
    IReadOnlyList<ErrorImportacion> ErroresCeldas,
    IReadOnlyList<ErrorImportacion> ErroresArchivo)
{
    public static LecturaPlantilla Rechazada(params ErrorImportacion[] errores) => new([], [], errores);
}
