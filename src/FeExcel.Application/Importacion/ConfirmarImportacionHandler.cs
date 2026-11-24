using FeExcel.Application.Abstracciones;

namespace FeExcel.Application.Importacion;

public enum EstadoConfirmacion
{
    Exitosa,

    /// <summary>La importación no existe: venció, ya se confirmó o el id es inválido.</summary>
    NoEncontrada,

    /// <summary>La importación tiene errores; se debe corregir el archivo y volver a subirlo.</summary>
    ConErrores
}

public sealed record ResultadoConfirmacion(EstadoConfirmacion Estado, int CantidadEnviada = 0);

/// <summary>
/// Caso de uso "confirmar": entrega todos los comprobantes de una importación sin errores al repositorio.
/// Es todo o nada: si hay un solo error, no se envía nada.
/// </summary>
public sealed class ConfirmarImportacionHandler(IAlmacenImportaciones almacen, IComprobanteRepository repositorio)
{
    public async Task<ResultadoConfirmacion> EjecutarAsync(Guid importacionId, CancellationToken cancellationToken)
    {
        var importacion = almacen.Obtener(importacionId);
        if (importacion is null)
            return new ResultadoConfirmacion(EstadoConfirmacion.NoEncontrada);

        if (importacion.TieneErrores)
            return new ResultadoConfirmacion(EstadoConfirmacion.ConErrores);

        // Se retira antes de guardar para que una doble confirmación no envíe los comprobantes dos veces.
        var retirada = almacen.Retirar(importacionId);
        if (retirada is null)
            return new ResultadoConfirmacion(EstadoConfirmacion.NoEncontrada);

        var comprobantes = retirada.ComprobantesValidos;

        try
        {
            await repositorio.GuardarAsync(comprobantes, cancellationToken);
        }
        catch
        {
            // Si falla la persistencia, la importación vuelve al almacén para poder reintentar sin subir otra vez el archivo.
            almacen.Guardar(retirada);
            throw;
        }

        return new ResultadoConfirmacion(EstadoConfirmacion.Exitosa, comprobantes.Count);
    }
}
