using FeExcel.Application.Plantilla;
using FeExcel.Domain.Comprobantes;
using FeExcel.Domain.Errores;

namespace FeExcel.Application.Importacion;

/// <summary>
/// Campos que cada tipo de e-CF exige antes de pasar el comprobante a la librería DGII.
/// La librería no valida todos estos casos, o los reporta con mensajes poco claros;
/// por eso se revisan aquí primero. Las columnas que no aplican a un tipo se ignoran.
/// </summary>
public sealed class ReglasPorTipoEcf
{
    private static readonly IReadOnlyDictionary<TipoEcf, ColumnaPlantilla[]> RequeridosPorTipo =
        new Dictionary<TipoEcf, ColumnaPlantilla[]>
        {
            [TipoEcf.FacturaCreditoFiscal] =
                [ColumnasPlantilla.TipoPago, ColumnasPlantilla.TipoIngreso, ColumnasPlantilla.FechaVencimientoSecuencia, ColumnasPlantilla.RNCComprador, ColumnasPlantilla.ArticulosIncluyenITBIS],
            [TipoEcf.FacturaConsumo] =
                [ColumnasPlantilla.TipoPago, ColumnasPlantilla.TipoIngreso, ColumnasPlantilla.ArticulosIncluyenITBIS],
            [TipoEcf.NotaDebito] =
                [ColumnasPlantilla.TipoPago, ColumnasPlantilla.TipoIngreso, ColumnasPlantilla.FechaVencimientoSecuencia, .. CamposReferencia, ColumnasPlantilla.ArticulosIncluyenITBIS],
            [TipoEcf.NotaCredito] =
                [ColumnasPlantilla.TipoPago, ColumnasPlantilla.TipoIngreso, .. CamposReferencia, ColumnasPlantilla.ArticulosIncluyenITBIS],
            [TipoEcf.Compras] =
                [ColumnasPlantilla.TipoPago, ColumnasPlantilla.FechaVencimientoSecuencia, ColumnasPlantilla.ArticulosIncluyenITBIS],
            [TipoEcf.GastosMenores] =
                [ColumnasPlantilla.FechaVencimientoSecuencia],
            [TipoEcf.RegimenesEspeciales] =
                [ColumnasPlantilla.TipoPago, ColumnasPlantilla.FechaVencimientoSecuencia],
            [TipoEcf.Gubernamental] =
                [ColumnasPlantilla.TipoPago, ColumnasPlantilla.FechaVencimientoSecuencia, ColumnasPlantilla.ArticulosIncluyenITBIS],
            [TipoEcf.Exportaciones] =
                [ColumnasPlantilla.TipoPago, ColumnasPlantilla.TipoIngreso, ColumnasPlantilla.FechaVencimientoSecuencia],
            [TipoEcf.PagosExterior] =
                [ColumnasPlantilla.TipoPago, ColumnasPlantilla.FechaVencimientoSecuencia],
        };

    private static ColumnaPlantilla[] CamposReferencia =>
        [ColumnasPlantilla.NCFModificado, ColumnasPlantilla.FechaNCFModificado, ColumnasPlantilla.CodigoModificacion];

    public IReadOnlyList<ErrorImportacion> Validar(TipoEcf tipo, GrupoComprobante grupo)
    {
        var errores = new List<ErrorImportacion>();
        var encabezado = grupo.Encabezado;

        ValidarEncabezadoRequerido(tipo, encabezado, errores);
        ValidarMoneda(encabezado, errores);
        ValidarComprador(encabezado, errores);
        ValidarDetalles(grupo, errores);

        return errores;
    }

    private static void ValidarEncabezadoRequerido(TipoEcf tipo, EncabezadoFila encabezado, List<ErrorImportacion> errores)
    {
        IEnumerable<ColumnaPlantilla> requeridos = [ColumnasPlantilla.FechaEmision, .. RequeridosPorTipo[tipo]];

        foreach (var columna in requeridos.Where(c => !CamposEncabezado.TieneValor(encabezado, c.Nombre)))
        {
            errores.Add(new ErrorImportacion(null, columna.Nombre,
                $"El campo '{columna.Nombre}' es requerido para el tipo {tipo.Codigo()}"));
        }
    }

    private static void ValidarMoneda(EncabezadoFila encabezado, List<ErrorImportacion> errores)
    {
        var tieneMoneda = encabezado.TipoMoneda is not null;
        var tieneCambio = encabezado.TipoCambio is not null;

        if (tieneMoneda && !tieneCambio)
            errores.Add(new ErrorImportacion(null, ColumnasPlantilla.TipoCambio.Nombre,
                "Si se indica 'TipoMoneda' también debe indicarse 'TipoCambio'"));

        if (tieneCambio && !tieneMoneda)
            errores.Add(new ErrorImportacion(null, ColumnasPlantilla.TipoMoneda.Nombre,
                "Si se indica 'TipoCambio' también debe indicarse 'TipoMoneda'"));
    }

    private static void ValidarComprador(EncabezadoFila encabezado, List<ErrorImportacion> errores)
    {
        if (encabezado.RNCComprador is not null && encabezado.IdentificadorExtranjero is not null)
            errores.Add(new ErrorImportacion(null, ColumnasPlantilla.IdentificadorExtranjero.Nombre,
                "No se puede indicar 'RNCComprador' e 'IdentificadorExtranjero' a la vez"));
    }

    private static void ValidarDetalles(GrupoComprobante grupo, List<ErrorImportacion> errores)
    {
        foreach (var fila in grupo.Filas)
        {
            var detalle = fila.Detalle;
            var campos = new (ColumnaPlantilla Columna, object? Valor)[]
            {
                (ColumnasPlantilla.NombreItem, detalle.NombreItem),
                (ColumnasPlantilla.BienServicio, detalle.BienServicio),
                (ColumnasPlantilla.TipoITBIS, detalle.TipoITBIS),
                (ColumnasPlantilla.Cantidad, detalle.Cantidad),
                (ColumnasPlantilla.PrecioUnitario, detalle.PrecioUnitario),
            };

            foreach (var campo in campos.Where(c => c.Valor is null))
                errores.Add(ErrorImportacion.EnCelda(fila.NumeroFila, campo.Columna.Nombre, "El valor es requerido"));
        }
    }
}
