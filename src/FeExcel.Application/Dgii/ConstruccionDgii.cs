using DGII.FE.XML.Models.Externo;
using DGII.FE.XML.Models.Externo.Compradores;
using DGII.FE.XML.Models.Externo.Comprobantes;
using DGII.FE.XML.Models.Externo.Detalles;
using DGII.FE.XML.Models.Externo.InformacionesReferencia;
using DGII.FE.XML.Models.Externo.Retenciones;
using FeExcel.Application.Importacion;
using FeExcel.Application.Plantilla;

namespace FeExcel.Application.Dgii;

/// <summary>Piezas comunes a todos los constructores de comprobantes DGII.</summary>
internal static class ConstruccionDgii
{
    /// <summary>Asigna los datos que comparten todos los tipos: secuencia, emisor, fecha y moneda.</summary>
    public static T ConDatosComunes<T>(T comprobante, GrupoComprobante grupo, Emisor emisor) where T : Comprobante
    {
        var encabezado = grupo.Encabezado;

        comprobante.Secuencia = grupo.Encf;
        comprobante.Emisor = emisor;
        comprobante.FechaEmision = encabezado.FechaEmision!.Value;

        // Los setters de TipoMoneda y TipoCambio se validan entre sí y dependen del orden:
        // se asignan solo cuando vienen ambos (ReglasPorTipoEcf ya verificó que vengan juntos).
        if (encabezado.TipoMoneda is not null && encabezado.TipoCambio is not null)
        {
            comprobante.TipoMoneda = encabezado.TipoMoneda;
            comprobante.TipoCambio = encabezado.TipoCambio;
        }

        return comprobante;
    }

    /// <summary>Comprador nacional o extranjero según la columna que venga; <c>null</c> si no hay comprador.</summary>
    public static Comprador? Comprador(EncabezadoFila encabezado)
    {
        if (encabezado.RNCComprador is not null)
        {
            return new CompradorNacional
            {
                RNCComprador = encabezado.RNCComprador,
                RazonSocialComprador = encabezado.RazonSocialComprador
            };
        }

        if (encabezado.IdentificadorExtranjero is not null)
        {
            return new CompradorExtranjero
            {
                IdentificadorExtranjero = encabezado.IdentificadorExtranjero,
                RazonSocialComprador = encabezado.RazonSocialComprador
            };
        }

        return null;
    }

    public static InformacionReferencia Referencia(EncabezadoFila encabezado) => new()
    {
        NCFModificado = encabezado.NCFModificado,
        FechaNCFModificado = encabezado.FechaNCFModificado,
        CodigoModificacion = encabezado.CodigoModificacion
    };

    public static Retencion Retencion(DetalleFila detalle) => new()
    {
        AgentePercepcionRetencion = detalle.IndicadorAgenteRetencion,
        MontoITBISRetenido = detalle.MontoITBISRetenido,
        MontoISRRetenido = detalle.MontoISRRetenido
    };

    /// <inheritdoc cref="Detalles{TDetalle}(GrupoComprobante, Func{FilaComprobante, TDetalle})"/>
    public static List<TDetalle> Detalles<TDetalle>(GrupoComprobante grupo) where TDetalle : Detalle, new() =>
        Detalles(grupo, _ => new TDetalle());

    /// <summary>
    /// Una línea de detalle por fila. <paramref name="crear"/> instancia el detalle con los campos propios
    /// de su tipo (por ejemplo, la retención); aquí se asignan los campos comunes.
    /// Si la librería rechaza un valor, la excepción se envuelve en <see cref="ExcepcionEnFila"/>
    /// para poder indicar la fila del Excel.
    /// </summary>
    public static List<TDetalle> Detalles<TDetalle>(GrupoComprobante grupo, Func<FilaComprobante, TDetalle> crear)
        where TDetalle : Detalle =>
        grupo.Filas.Select(fila =>
        {
            try
            {
                var detalle = crear(fila);
                detalle.Nombre = fila.Detalle.NombreItem!;
                detalle.BienServicio = fila.Detalle.BienServicio;
                detalle.TipoITBIS = fila.Detalle.TipoITBIS;
                detalle.Cantidad = fila.Detalle.Cantidad!.Value;
                detalle.PrecioUnitario = fila.Detalle.PrecioUnitario!.Value;
                return detalle;
            }
            catch (Exception ex)
            {
                throw new ExcepcionEnFila(fila.NumeroFila, ex);
            }
        }).ToList();
}

/// <summary>Excepción de la librería DGII ocurrida al construir el detalle de una fila concreta.</summary>
internal sealed class ExcepcionEnFila(int fila, Exception inner) : Exception(inner.Message, inner)
{
    public int Fila { get; } = fila;
}
