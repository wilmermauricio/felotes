using ClosedXML.Excel;
using FeExcel.Application.Abstracciones;
using FeExcel.Application.Plantilla;
using FeExcel.Domain.Errores;

namespace FeExcel.Infrastructure.Excel;

/// <summary>
/// Lee la primera hoja del Excel. Las columnas se localizan por el texto del encabezado (fila 1),
/// no por su posición; los encabezados desconocidos se ignoran.
/// </summary>
public sealed class LectorPlantillaClosedXml : ILectorPlantilla
{
    private const int FilaEncabezados = 1;

    public LecturaPlantilla Leer(Stream archivo)
    {
        XLWorkbook libro;
        try
        {
            libro = new XLWorkbook(archivo);
        }
        catch (Exception)
        {
            return LecturaPlantilla.Rechazada(ErrorImportacion.General("No se pudo leer el archivo: no es un Excel .xlsx válido"));
        }

        using (libro)
        {
            var hoja = libro.Worksheets.First();
            var columnas = LocalizarColumnas(hoja.Row(FilaEncabezados));

            var faltantes = ColumnasPlantilla.Todas
                .Where(c => c.ObligatoriaEnArchivo && !columnas.ContainsKey(c.Nombre))
                .Select(c => ErrorImportacion.General($"Falta la columna obligatoria '{c.Nombre}'"))
                .ToArray();

            if (faltantes.Length > 0)
                return LecturaPlantilla.Rechazada(faltantes);

            return LeerFilas(hoja, columnas);
        }
    }

    /// <summary>Nombre de columna → número de columna en la hoja, solo para las columnas conocidas.</summary>
    private static Dictionary<string, int> LocalizarColumnas(IXLRow encabezados)
    {
        var conocidas = ColumnasPlantilla.Todas.ToDictionary(c => c.Nombre, c => c.Nombre, StringComparer.OrdinalIgnoreCase);
        var columnas = new Dictionary<string, int>();

        foreach (var celda in encabezados.CellsUsed())
        {
            var encabezado = celda.GetFormattedString().Trim();
            if (conocidas.TryGetValue(encabezado, out var nombre) && !columnas.ContainsKey(nombre))
                columnas[nombre] = celda.Address.ColumnNumber;
        }

        return columnas;
    }

    private static LecturaPlantilla LeerFilas(IXLWorksheet hoja, IReadOnlyDictionary<string, int> columnas)
    {
        var filas = new List<FilaComprobante>();
        var errores = new List<ErrorImportacion>();
        var ultimaFila = hoja.LastRowUsed()?.RowNumber() ?? FilaEncabezados;

        for (var numero = FilaEncabezados + 1; numero <= ultimaFila; numero++)
        {
            var celdas = new LectorCeldas(hoja.Row(numero), columnas, errores);
            if (!celdas.EstaVacia())
                filas.Add(LeerFila(celdas));
        }

        return new LecturaPlantilla(filas, errores, []);
    }

    private static FilaComprobante LeerFila(LectorCeldas celdas) => new(
        NumeroFila: celdas.NumeroFila,
        Empresa: celdas.Texto(ColumnasPlantilla.Empresa),
        ENCF: celdas.Texto(ColumnasPlantilla.ENCF),
        Encabezado: new EncabezadoFila
        {
            FechaEmision = celdas.Fecha(ColumnasPlantilla.FechaEmision),
            FechaVencimientoSecuencia = celdas.Fecha(ColumnasPlantilla.FechaVencimientoSecuencia),
            TipoPago = celdas.Entero(ColumnasPlantilla.TipoPago),
            TipoIngreso = celdas.Entero(ColumnasPlantilla.TipoIngreso),
            FechaLimitePago = celdas.Fecha(ColumnasPlantilla.FechaLimitePago),
            ArticulosIncluyenITBIS = celdas.Booleano(ColumnasPlantilla.ArticulosIncluyenITBIS),
            RNCComprador = celdas.Texto(ColumnasPlantilla.RNCComprador),
            IdentificadorExtranjero = celdas.Texto(ColumnasPlantilla.IdentificadorExtranjero),
            RazonSocialComprador = celdas.Texto(ColumnasPlantilla.RazonSocialComprador),
            NCFModificado = celdas.Texto(ColumnasPlantilla.NCFModificado),
            FechaNCFModificado = celdas.Fecha(ColumnasPlantilla.FechaNCFModificado),
            CodigoModificacion = celdas.Entero(ColumnasPlantilla.CodigoModificacion),
            TipoMoneda = celdas.Texto(ColumnasPlantilla.TipoMoneda),
            TipoCambio = celdas.Decimal(ColumnasPlantilla.TipoCambio)
        },
        Detalle: new DetalleFila
        {
            NombreItem = celdas.Texto(ColumnasPlantilla.NombreItem),
            BienServicio = celdas.Entero(ColumnasPlantilla.BienServicio),
            TipoITBIS = celdas.Entero(ColumnasPlantilla.TipoITBIS),
            Cantidad = celdas.Decimal(ColumnasPlantilla.Cantidad),
            PrecioUnitario = celdas.Decimal(ColumnasPlantilla.PrecioUnitario),
            IndicadorAgenteRetencion = celdas.Entero(ColumnasPlantilla.IndicadorAgenteRetencion),
            MontoITBISRetenido = celdas.Decimal(ColumnasPlantilla.MontoITBISRetenido),
            MontoISRRetenido = celdas.Decimal(ColumnasPlantilla.MontoISRRetenido)
        });
}
