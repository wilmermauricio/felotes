using FeExcel.Application.Dgii;
using FeExcel.Application.Plantilla;
using FeExcel.Domain.Comprobantes;

namespace FeExcel.Application.Importacion;

/// <summary>Convierte un grupo de filas ya validado en el objeto de dominio que se entrega al repositorio.</summary>
internal static class MapeadorComprobanteImportado
{
    public static ComprobanteImportado Mapear(TipoEcf tipo, GrupoComprobante grupo, ResultadoValidacionDgii validacion)
    {
        var encabezado = grupo.Encabezado;

        return new ComprobanteImportado
        {
            Empresa = grupo.Empresa,
            Encf = grupo.Encf,
            TipoEcf = tipo,
            FechaEmision = encabezado.FechaEmision!.Value,
            FechaVencimientoSecuencia = encabezado.FechaVencimientoSecuencia,
            TipoPago = encabezado.TipoPago,
            TipoIngreso = encabezado.TipoIngreso,
            FechaLimitePago = encabezado.FechaLimitePago,
            ArticulosIncluyenItbis = encabezado.ArticulosIncluyenITBIS,
            TipoMoneda = encabezado.TipoMoneda,
            TipoCambio = encabezado.TipoCambio,
            Comprador = MapearComprador(encabezado),
            Referencia = MapearReferencia(tipo, encabezado),
            Detalles = grupo.Filas.Select((fila, indice) => MapearDetalle(fila, indice, validacion.MontosPorLinea[indice])).ToList(),
            Totales = validacion.Totales!
        };
    }

    private static CompradorImportado? MapearComprador(EncabezadoFila encabezado)
    {
        if (encabezado.RNCComprador is null && encabezado.IdentificadorExtranjero is null && encabezado.RazonSocialComprador is null)
            return null;

        return new CompradorImportado
        {
            RncComprador = encabezado.RNCComprador,
            IdentificadorExtranjero = encabezado.IdentificadorExtranjero,
            RazonSocial = encabezado.RazonSocialComprador
        };
    }

    private static ReferenciaImportada? MapearReferencia(TipoEcf tipo, EncabezadoFila encabezado)
    {
        if (tipo is not (TipoEcf.NotaDebito or TipoEcf.NotaCredito))
            return null;

        return new ReferenciaImportada
        {
            NcfModificado = encabezado.NCFModificado!,
            FechaNcfModificado = encabezado.FechaNCFModificado!.Value,
            CodigoModificacion = encabezado.CodigoModificacion!.Value
        };
    }

    private static DetalleImportado MapearDetalle(FilaComprobante fila, int indice, decimal montoItem) => new()
    {
        NumeroLinea = indice + 1,
        FilaExcel = fila.NumeroFila,
        NombreItem = fila.Detalle.NombreItem!,
        BienServicio = fila.Detalle.BienServicio!.Value,
        TipoItbis = fila.Detalle.TipoITBIS!.Value,
        Cantidad = fila.Detalle.Cantidad!.Value,
        PrecioUnitario = fila.Detalle.PrecioUnitario!.Value,
        MontoItem = montoItem,
        IndicadorAgenteRetencion = fila.Detalle.IndicadorAgenteRetencion,
        MontoItbisRetenido = fila.Detalle.MontoITBISRetenido,
        MontoIsrRetenido = fila.Detalle.MontoISRRetenido
    };
}
