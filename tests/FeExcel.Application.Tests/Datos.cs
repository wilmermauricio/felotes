using FeExcel.Application.Importacion;
using FeExcel.Application.Plantilla;

namespace FeExcel.Application.Tests;

/// <summary>Datos de prueba: filas y encabezados válidos que cada prueba modifica con <c>with</c>.</summary>
internal static class Datos
{
    public static readonly EncabezadoFila EncabezadoCompleto = new()
    {
        FechaEmision = new DateTime(2026, 11, 20),
        FechaVencimientoSecuencia = new DateTime(2027, 12, 31),
        TipoPago = 1,
        TipoIngreso = 1,
        ArticulosIncluyenITBIS = false,
        RNCComprador = "131234567",
        RazonSocialComprador = "Cliente SRL",
        NCFModificado = "E310000000099",
        FechaNCFModificado = new DateTime(2026, 11, 1),
        CodigoModificacion = 3
    };

    public static readonly DetalleFila DetalleValido = new()
    {
        NombreItem = "Producto",
        BienServicio = 1,
        TipoITBIS = 1,
        Cantidad = 2,
        PrecioUnitario = 100
    };

    public static FilaComprobante Fila(
        int numero,
        string? empresa = "EMP01",
        string? encf = "E310000000001",
        EncabezadoFila? encabezado = null,
        DetalleFila? detalle = null) =>
        new(numero, empresa, encf, encabezado ?? EncabezadoCompleto, detalle ?? DetalleValido);

    public static GrupoComprobante Grupo(string encf, params FilaComprobante[] filas) =>
        new("EMP01", encf, filas);

    public static GrupoComprobante Grupo(string encf, EncabezadoFila encabezado) =>
        Grupo(encf, Fila(2, encf: encf, encabezado: encabezado));
}
