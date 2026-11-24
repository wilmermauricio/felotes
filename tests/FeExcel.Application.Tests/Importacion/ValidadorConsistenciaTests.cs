using FeExcel.Application.Importacion;
using static FeExcel.Application.Tests.Datos;

namespace FeExcel.Application.Tests.Importacion;

public class ValidadorConsistenciaTests
{
    private readonly ValidadorConsistencia _validador = new();

    [Fact]
    public void Fecha_de_emision_distinta_en_el_mismo_comprobante_es_inconsistencia()
    {
        var grupo = Grupo("E310000000001",
            Fila(2),
            Fila(3, encabezado: EncabezadoCompleto with { FechaEmision = new DateTime(2026, 11, 21) }));

        var error = Assert.Single(_validador.Validar(grupo));

        Assert.Equal("FechaEmision", error.Campo);
        Assert.Equal("Inconsistencia en 'FechaEmision': las filas 2 y 3 tienen valores distintos", error.Mensaje);
    }

    [Fact]
    public void Reporta_todas_las_filas_en_conflicto()
    {
        var otroComprador = EncabezadoCompleto with { RNCComprador = "101010101" };
        var grupo = Grupo("E310000000001", Fila(2), Fila(3, encabezado: otroComprador), Fila(5, encabezado: otroComprador));

        var error = Assert.Single(_validador.Validar(grupo));

        Assert.Equal("Inconsistencia en 'RNCComprador': las filas 2, 3 y 5 tienen valores distintos", error.Mensaje);
    }

    [Fact]
    public void Encabezado_repetido_igual_no_genera_errores()
    {
        var grupo = Grupo("E310000000001", Fila(2), Fila(3, detalle: DetalleValido with { NombreItem = "Otro" }));

        Assert.Empty(_validador.Validar(grupo));
    }
}
