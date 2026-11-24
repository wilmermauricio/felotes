using FeExcel.Application.Importacion;
using FeExcel.Domain.Comprobantes;
using static FeExcel.Application.Tests.Datos;

namespace FeExcel.Application.Tests.Importacion;

public class ReglasPorTipoEcfTests
{
    private readonly ReglasPorTipoEcf _reglas = new();

    [Fact]
    public void Credito_fiscal_sin_rnc_del_comprador_es_error()
    {
        var grupo = Grupo("E310000000001", EncabezadoCompleto with { RNCComprador = null });

        var error = Assert.Single(_reglas.Validar(TipoEcf.FacturaCreditoFiscal, grupo));

        Assert.Equal("El campo 'RNCComprador' es requerido para el tipo 31", error.Mensaje);
    }

    [Theory]
    [InlineData(TipoEcf.FacturaCreditoFiscal, "E310000000001")]
    [InlineData(TipoEcf.FacturaConsumo, "E320000000001")]
    [InlineData(TipoEcf.NotaDebito, "E330000000001")]
    [InlineData(TipoEcf.NotaCredito, "E340000000001")]
    [InlineData(TipoEcf.Compras, "E410000000001")]
    [InlineData(TipoEcf.Gubernamental, "E450000000001")]
    public void Tipos_con_itbis_exigen_indicar_si_el_precio_lo_incluye(TipoEcf tipo, string encf)
    {
        var grupo = Grupo(encf, EncabezadoCompleto with { ArticulosIncluyenITBIS = null });

        var error = Assert.Single(_reglas.Validar(tipo, grupo));

        Assert.Equal($"El campo 'ArticulosIncluyenITBIS' es requerido para el tipo {tipo.Codigo()}", error.Mensaje);
    }

    [Fact]
    public void Moneda_sin_tasa_es_error()
    {
        var grupo = Grupo("E310000000001", EncabezadoCompleto with { TipoMoneda = "USD" });

        var error = Assert.Single(_reglas.Validar(TipoEcf.FacturaCreditoFiscal, grupo));

        Assert.Equal("Si se indica 'TipoMoneda' también debe indicarse 'TipoCambio'", error.Mensaje);
    }

    [Fact]
    public void Rnc_e_identificador_extranjero_juntos_es_error()
    {
        var grupo = Grupo("E320000000001", EncabezadoCompleto with { IdentificadorExtranjero = "P123" });

        var error = Assert.Single(_reglas.Validar(TipoEcf.FacturaConsumo, grupo));

        Assert.Equal("No se puede indicar 'RNCComprador' e 'IdentificadorExtranjero' a la vez", error.Mensaje);
    }

    [Fact]
    public void Columnas_que_no_aplican_al_tipo_se_ignoran()
    {
        var gastosMenores = new Plantilla.EncabezadoFila
        {
            FechaEmision = new DateTime(2026, 11, 20),
            FechaVencimientoSecuencia = new DateTime(2027, 12, 31),
            RNCComprador = "131234567"
        };

        Assert.Empty(_reglas.Validar(TipoEcf.GastosMenores, Grupo("E430000000001", gastosMenores)));
    }

    [Fact]
    public void Notas_exigen_la_referencia_al_comprobante_modificado()
    {
        var grupo = Grupo("E340000000001", EncabezadoCompleto with { NCFModificado = null, CodigoModificacion = null });

        var campos = _reglas.Validar(TipoEcf.NotaCredito, grupo).Select(e => e.Campo);

        Assert.Equal(["NCFModificado", "CodigoModificacion"], campos);
    }

    [Fact]
    public void Detalle_sin_cantidad_es_error_en_su_fila()
    {
        var grupo = Grupo("E310000000001", Fila(2), Fila(3, detalle: DetalleValido with { Cantidad = null }));

        var error = Assert.Single(_reglas.Validar(TipoEcf.FacturaCreditoFiscal, grupo));

        Assert.Equal("Fila 3, columna 'Cantidad': El valor es requerido", error.ToString());
    }
}
