using FeExcel.Application.Importacion;
using static FeExcel.Application.Tests.Datos;

namespace FeExcel.Application.Tests.Importacion;

public class AgrupadorComprobantesTests
{
    private readonly AgrupadorComprobantes _agrupador = new();

    [Fact]
    public void Filas_con_misma_empresa_y_encf_forman_un_comprobante()
    {
        var resultado = _agrupador.Agrupar([Fila(2), Fila(3)]);

        var grupo = Assert.Single(resultado.Grupos);
        Assert.Equal([2, 3], grupo.NumerosFila);
    }

    [Fact]
    public void Mismo_encf_en_empresas_distintas_son_comprobantes_distintos()
    {
        var resultado = _agrupador.Agrupar([Fila(2, empresa: "EMP01"), Fila(3, empresa: "EMP02")]);

        Assert.Equal(2, resultado.Grupos.Count);
    }

    [Fact]
    public void Filas_no_contiguas_del_mismo_comprobante_se_agrupan()
    {
        var resultado = _agrupador.Agrupar(
        [
            Fila(2, encf: "E310000000001"),
            Fila(3, encf: "E310000000002"),
            Fila(9, encf: "E310000000001")
        ]);

        Assert.Equal(2, resultado.Grupos.Count);
        Assert.Equal([2, 9], resultado.Grupos[0].NumerosFila);
    }

    [Fact]
    public void La_clave_ignora_mayusculas_y_espacios()
    {
        var resultado = _agrupador.Agrupar([Fila(2, empresa: "emp01", encf: " e310000000001 "), Fila(3)]);

        var grupo = Assert.Single(resultado.Grupos);
        Assert.Equal("E310000000001", grupo.Encf);
    }

    [Fact]
    public void Fila_sin_empresa_genera_error_y_no_se_agrupa()
    {
        var resultado = _agrupador.Agrupar([Fila(2, empresa: null), Fila(3)]);

        Assert.Single(resultado.Grupos);
        var error = Assert.Single(resultado.Errores);
        Assert.Equal("Fila 2, columna 'Empresa': El valor es requerido", error.ToString());
    }
}
