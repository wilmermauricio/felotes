using System.Security.Cryptography.X509Certificates;
using DGII.FE.Models.Interno;
using DGII.FE.Services.Interfaces;
using DGII.FE.XML;
using DGII.FE.XML.Models.Externo;
using FeExcel.Application.Importacion;
using FeExcel.Domain.Comprobantes;
using FeExcel.Domain.Errores;

namespace FeExcel.Application.Dgii;

/// <summary>
/// Adaptador de la librería DGII: construye el modelo del tipo de e-CF, obtiene la
/// <see cref="FacturaInterna"/> (que valida y calcula los totales) y traduce sus excepciones a errores legibles.
/// </summary>
public sealed class ValidadorComprobantesDgii : IValidadorDgii
{
    private readonly IReadOnlyDictionary<TipoEcf, IConstructorComprobante> _constructores;
    private readonly FacturaFormatter _formatter = new(new ServicioFacturaNoUsado());

    public ValidadorComprobantesDgii(IEnumerable<IConstructorComprobante> constructores)
    {
        _constructores = constructores.ToDictionary(c => c.Tipo);
    }

    public ResultadoValidacionDgii Validar(TipoEcf tipo, GrupoComprobante grupo, Emisor emisor)
    {
        try
        {
            var comprobante = _constructores[tipo].Construir(grupo, emisor);
            var factura = _formatter.GetFacturaInterna(comprobante);

            return ResultadoValidacionDgii.Exitoso(ObtenerTotales(factura), ObtenerMontosPorLinea(factura));
        }
        catch (ExcepcionEnFila ex)
        {
            return ResultadoValidacionDgii.ConError(Traducir(ex.InnerException!, ex.Fila));
        }
        catch (Exception ex)
        {
            // Cualquier fallo de la librería invalida solo este comprobante; el resto del archivo se sigue procesando.
            return ResultadoValidacionDgii.ConError(Traducir(ex, fila: null));
        }
    }

    private static TotalesComprobante ObtenerTotales(FacturaInterna factura)
    {
        var total = factura.Total
            ?? throw new InvalidOperationException("La librería DGII no calculó los totales del comprobante.");

        return new TotalesComprobante
        {
            MontoGravadoTotal = total.MontoGravadoTotal,
            MontoExento = total.MontoExento,
            TotalItbis = total.TotalITBIS,
            MontoTotal = total.MontoTotal ?? 0m
        };
    }

    private static IReadOnlyList<decimal> ObtenerMontosPorLinea(FacturaInterna factura) =>
        factura.Detalles!.OrderBy(d => d.NumeroLinea).Select(d => d.MontoItem).ToList();

    /// <summary>
    /// Las excepciones de la librería traen el nombre del campo en <see cref="ArgumentException.ParamName"/>
    /// y el sufijo " (Parameter '...')" en el mensaje; se separan para mostrarlos limpios.
    /// </summary>
    private static ErrorImportacion Traducir(Exception ex, int? fila)
    {
        if (ex is ArgumentException { ParamName: { } campo } argumento && !campo.Contains(' '))
        {
            var mensaje = argumento.Message.Replace($" (Parameter '{campo}')", string.Empty);
            return new ErrorImportacion(fila, campo, mensaje);
        }

        var detalle = ex is NullReferenceException
            ? "La librería DGII encontró un dato faltante al procesar el comprobante"
            : ex.Message;

        return new ErrorImportacion(fila, null, detalle);
    }

    /// <summary>
    /// <see cref="FacturaFormatter"/> exige un <see cref="IFacturaService"/> aunque
    /// <see cref="FacturaFormatter.GetFacturaInterna"/> no lo use. Esta implementación existe solo
    /// para satisfacer ese constructor: aquí no se firma ni se genera XML.
    /// </summary>
    private sealed class ServicioFacturaNoUsado : IFacturaService
    {
        public FacturaFirmada ObtenerFacturaXMLFirmada(FacturaInterna factura, X509Certificate2 certificado, string xmlString) =>
            throw new NotSupportedException("El importador no firma comprobantes.");

        public FacturaFirmada ObtenerFacturaXMLFirmada(FacturaInterna factura, string xmlString, string rfce = null!) =>
            throw new NotSupportedException("El importador no firma comprobantes.");
    }
}
