using FeExcel.Application.Abstracciones;
using FeExcel.Application.Importacion;
using FeExcel.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace FeExcel.Web.Controllers;

/// <summary>Flujo de importación: subir el Excel → vista previa → confirmar.</summary>
public class ImportacionController(
    ProcesarArchivoHandler procesarArchivo,
    ConfirmarImportacionHandler confirmarImportacion,
    IAlmacenImportaciones almacen,
    IGeneradorPlantilla generadorPlantilla) : Controller
{
    private const string ClaveMensaje = "Mensaje";
    private const string TipoContenidoXlsx = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

    /// <summary>Margen sobre el tamaño del archivo para el resto del formulario multipart.</summary>
    private const long LimitePeticionBytes = ProcesarArchivoHandler.TamanoMaximoBytes + 1024 * 1024;

    [HttpGet]
    public IActionResult Index() => View();

    [HttpGet]
    public IActionResult DescargarPlantilla() =>
        File(generadorPlantilla.Generar(), TipoContenidoXlsx, "PlantillaComprobantes.xlsx");

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(LimitePeticionBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = LimitePeticionBytes)]
    public async Task<IActionResult> Subir(IFormFile? archivo, CancellationToken cancellationToken)
    {
        if (archivo is null)
        {
            TempData[ClaveMensaje] = "Seleccione un archivo .xlsx";
            return RedirectToAction(nameof(Index));
        }

        await using var contenido = archivo.OpenReadStream();
        var resultado = await procesarArchivo.EjecutarAsync(
            new ArchivoSubido(archivo.FileName, archivo.Length, contenido), cancellationToken);

        return RedirectToAction(nameof(VistaPrevia), new { id = resultado.Id });
    }

    [HttpGet]
    public IActionResult VistaPrevia(Guid id)
    {
        var resultado = almacen.Obtener(id);
        if (resultado is null)
            return VolverAlInicio("La importación expiró, vuelva a subir el archivo");

        return View(VistaPreviaViewModel.Desde(resultado));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Confirmar(Guid id, CancellationToken cancellationToken)
    {
        var resultado = await confirmarImportacion.EjecutarAsync(id, cancellationToken);

        return resultado.Estado switch
        {
            EstadoConfirmacion.Exitosa => View("Resultado", resultado.CantidadEnviada),
            EstadoConfirmacion.ConErrores => VolverAVistaPrevia(id, "Corrija los errores y vuelva a subir el archivo"),
            _ => VolverAlInicio("La importación expiró, vuelva a subir el archivo")
        };
    }

    /// <summary>Descarta la carga para que el usuario pueda subir el archivo corregido.</summary>
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Eliminar(Guid id)
    {
        var eliminada = almacen.Retirar(id);
        if (eliminada is null)
            return RedirectToAction(nameof(Index));

        return VolverAlInicio($"Se eliminó la carga del archivo '{eliminada.NombreArchivo}'");
    }

    private RedirectToActionResult VolverAlInicio(string mensaje)
    {
        TempData[ClaveMensaje] = mensaje;
        return RedirectToAction(nameof(Index));
    }

    private RedirectToActionResult VolverAVistaPrevia(Guid id, string mensaje)
    {
        TempData[ClaveMensaje] = mensaje;
        return RedirectToAction(nameof(VistaPrevia), new { id });
    }
}
