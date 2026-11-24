using FeExcel.Application.Dgii;
using FeExcel.Application.Dgii.Constructores;
using FeExcel.Application.Importacion;
using FeExcel.Application.Plantilla;
using FeExcel.Domain.Errores;
using static FeExcel.Application.Tests.Datos;

namespace FeExcel.Application.Tests.Importacion;

public class HandlersTests
{
    private readonly AlmacenEnMemoria _almacen = new();
    private readonly RepositorioEspia _repositorio = new();

    private ProcesarArchivoHandler CrearProcesador(LecturaPlantilla lectura, params string[] empresas) => new(
        new LectorFijo(lectura),
        new AgrupadorComprobantes(),
        new ProcesadorComprobante(
            new ValidadorConsistencia(),
            new ReglasPorTipoEcf(),
            new ValidadorComprobantesDgii([new ConstructorFacturaCreditoFiscal(), new ConstructorFacturaConsumo()])),
        new EmisoresFijos(empresas.Length > 0 ? empresas : ["EMP01"]),
        _almacen);

    private ConfirmarImportacionHandler CrearConfirmador() => new(_almacen, _repositorio);

    private static ArchivoSubido Archivo(string nombre = "comprobantes.xlsx") => new(nombre, 100, Stream.Null);

    private static LecturaPlantilla Lectura(params FilaComprobante[] filas) => new(filas, [], []);

    [Fact]
    public async Task Extension_no_permitida_rechaza_el_archivo()
    {
        var resultado = await CrearProcesador(Lectura(Fila(2))).EjecutarAsync(Archivo("datos.csv"), default);

        Assert.Equal("Solo se aceptan archivos .xlsx", Assert.Single(resultado.ErroresGenerales).Mensaje);
    }

    [Fact]
    public async Task Archivo_sin_filas_de_datos_se_rechaza()
    {
        var resultado = await CrearProcesador(Lectura()).EjecutarAsync(Archivo(), default);

        Assert.Equal("El archivo no contiene filas de datos", Assert.Single(resultado.ErroresGenerales).Mensaje);
    }

    [Fact]
    public async Task Archivo_valido_queda_disponible_para_la_vista_previa()
    {
        var resultado = await CrearProcesador(Lectura(Fila(2), Fila(3))).EjecutarAsync(Archivo(), default);

        Assert.False(resultado.TieneErrores);
        var comprobante = Assert.Single(resultado.ComprobantesValidos);
        Assert.Equal(2, comprobante.Detalles.Count);
        Assert.Equal(472m, comprobante.Totales.MontoTotal);
        Assert.Same(resultado, _almacen.Obtener(resultado.Id));
    }

    [Fact]
    public async Task Emisor_no_encontrado_invalida_los_comprobantes_de_esa_empresa()
    {
        var lectura = Lectura(Fila(2, empresa: "EMP01"), Fila(3, empresa: "EMP99"));

        var resultado = await CrearProcesador(lectura, "EMP01").EjecutarAsync(Archivo(), default);

        Assert.Equal(1, resultado.CantidadValidos);
        var invalido = Assert.Single(resultado.Comprobantes, c => !c.EsValido);
        Assert.Equal("No se encontró el emisor para la empresa 'EMP99'", Assert.Single(invalido.Errores).Mensaje);
    }

    [Fact]
    public async Task Errores_de_formato_se_asignan_al_comprobante_de_su_fila()
    {
        var errorCelda = ErrorImportacion.EnCelda(3, "Cantidad", "'abc' no es un número válido");
        var lectura = new LecturaPlantilla([Fila(2), Fila(3, detalle: DetalleValido with { Cantidad = null })], [errorCelda], []);

        var resultado = await CrearProcesador(lectura).EjecutarAsync(Archivo(), default);

        var comprobante = Assert.Single(resultado.Comprobantes);
        Assert.Equal([errorCelda], comprobante.Errores);
    }

    [Fact]
    public async Task Confirmar_una_importacion_vencida_no_llama_al_repositorio()
    {
        var resultado = await CrearConfirmador().EjecutarAsync(Guid.NewGuid(), default);

        Assert.Equal(EstadoConfirmacion.NoEncontrada, resultado.Estado);
        Assert.Empty(_repositorio.Llamadas);
    }

    [Fact]
    public async Task Confirmar_con_errores_se_rechaza()
    {
        var importacion = await CrearProcesador(Lectura(Fila(2), Fila(3, encf: "E990000000001"))).EjecutarAsync(Archivo(), default);

        var resultado = await CrearConfirmador().EjecutarAsync(importacion.Id, default);

        Assert.Equal(EstadoConfirmacion.ConErrores, resultado.Estado);
        Assert.Empty(_repositorio.Llamadas);
    }

    [Fact]
    public async Task Doble_confirmacion_envia_los_comprobantes_una_sola_vez()
    {
        var importacion = await CrearProcesador(Lectura(Fila(2), Fila(3, encf: "E310000000002"))).EjecutarAsync(Archivo(), default);
        var confirmador = CrearConfirmador();

        var primera = await confirmador.EjecutarAsync(importacion.Id, default);
        var segunda = await confirmador.EjecutarAsync(importacion.Id, default);

        Assert.Equal(new ResultadoConfirmacion(EstadoConfirmacion.Exitosa, 2), primera);
        Assert.Equal(EstadoConfirmacion.NoEncontrada, segunda.Estado);
        Assert.Equal(2, Assert.Single(_repositorio.Llamadas).Count);
    }
}
