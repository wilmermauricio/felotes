using FeExcel.Domain.Comprobantes;

namespace FeExcel.Application.Tests.Importacion;

public class TipoEcfTests
{
    [Theory]
    [InlineData("E310000000001", TipoEcf.FacturaCreditoFiscal)]
    [InlineData("E320000000001", TipoEcf.FacturaConsumo)]
    [InlineData("E470000000001", TipoEcf.PagosExterior)]
    public void Deduce_el_tipo_del_encf(string encf, TipoEcf esperado)
    {
        Assert.True(TipoEcfExtensions.TryDesdeEncf(encf, out var tipo, out _));
        Assert.Equal(esperado, tipo);
    }

    [Fact]
    public void Tipo_no_soportado()
    {
        Assert.False(TipoEcfExtensions.TryDesdeEncf("E990000000001", out _, out var error));
        Assert.Equal("Tipo de e-CF '99' no soportado", error);
    }

    [Theory]
    [InlineData("E31000000001")]
    [InlineData("B310000000001")]
    [InlineData("E31000000000A")]
    public void Formato_invalido(string encf)
    {
        Assert.False(TipoEcfExtensions.TryDesdeEncf(encf, out _, out var error));
        Assert.StartsWith($"El ENCF '{encf}' no es válido", error);
    }
}
