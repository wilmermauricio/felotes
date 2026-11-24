using DGII.FE.XML.Models.Externo;
using FeExcel.Application.Dgii;
using FeExcel.Application.Dgii.Constructores;
using FeExcel.Domain.Comprobantes;
using static FeExcel.Application.Tests.Datos;

namespace FeExcel.Application.Tests.Dgii;

/// <summary>Pruebas contra la librería DGII real.</summary>
public class ValidadorComprobantesDgiiTests
{
    private static readonly Emisor Emisor = new()
    {
        RNCEmisor = "131234567",
        RazonSocialEmisor = "Emisor de Prueba SRL",
        DireccionEmisor = "Calle 1",
        Municipio = "010100",
        Provincia = "010000"
    };

    private readonly ValidadorComprobantesDgii _validador = new(
    [
        new ConstructorFacturaCreditoFiscal(), new ConstructorFacturaConsumo(), new ConstructorNotaDebito(),
        new ConstructorNotaCredito(), new ConstructorCompras(), new ConstructorGastosMenores(),
        new ConstructorRegimenesEspeciales(), new ConstructorGubernamental(), new ConstructorExportaciones(),
        new ConstructorPagosExterior()
    ]);

    public static TheoryData<TipoEcf> TodosLosTipos => new(Enum.GetValues<TipoEcf>());

    [Theory]
    [MemberData(nameof(TodosLosTipos))]
    public void Comprobante_valido_de_cada_tipo_produce_totales(TipoEcf tipo)
    {
        var encf = $"E{tipo.Codigo()}0000000001";

        var resultado = _validador.Validar(tipo, Grupo(encf, Fila(2, encf: encf), Fila(3, encf: encf)), Emisor);

        Assert.True(resultado.EsValido, string.Join("; ", resultado.Errores));
        Assert.True(resultado.Totales!.MontoTotal > 0);
        Assert.Equal(2, resultado.MontosPorLinea.Count);
    }

    [Fact]
    public void Calcula_el_itbis_al_18_por_ciento_sobre_precios_sin_itbis()
    {
        var resultado = _validador.Validar(TipoEcf.FacturaCreditoFiscal, Grupo("E310000000001", Fila(2)), Emisor);

        Assert.Equal(200m, resultado.MontosPorLinea[0]);
        Assert.Equal(36m, resultado.Totales!.TotalItbis);
        Assert.Equal(236m, resultado.Totales.MontoTotal);
    }

    [Fact]
    public void Valor_rechazado_por_la_libreria_es_un_error_legible_con_su_fila()
    {
        var grupo = Grupo("E310000000001", Fila(2), Fila(3, detalle: DetalleValido with { BienServicio = 3 }));

        var resultado = _validador.Validar(TipoEcf.FacturaCreditoFiscal, grupo, Emisor);

        var error = Assert.Single(resultado.Errores);
        Assert.Equal(3, error.Fila);
        Assert.Equal("BienServicio", error.Campo);
        Assert.Equal("El valor de IndicadorBienoServicio debe ser 1 (Bien) o 2 (Servicio).", error.Mensaje);
    }
}
