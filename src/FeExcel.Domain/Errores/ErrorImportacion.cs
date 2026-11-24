namespace FeExcel.Domain.Errores;

/// <summary>
/// Error encontrado al importar. <see cref="Fila"/> y <see cref="Campo"/> son opcionales:
/// un error de archivo no tiene fila, y un error de comprobante puede no tener campo.
/// </summary>
public sealed record ErrorImportacion(int? Fila, string? Campo, string Mensaje)
{
    public static ErrorImportacion General(string mensaje) => new(null, null, mensaje);

    public static ErrorImportacion EnFila(int fila, string mensaje) => new(fila, null, mensaje);

    public static ErrorImportacion EnCelda(int fila, string campo, string mensaje) => new(fila, campo, mensaje);

    public override string ToString() => (Fila, Campo) switch
    {
        (not null, not null) => $"Fila {Fila}, columna '{Campo}': {Mensaje}",
        (not null, null) => $"Fila {Fila}: {Mensaje}",
        _ => Mensaje
    };
}
