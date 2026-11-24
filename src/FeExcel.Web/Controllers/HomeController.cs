using System.Diagnostics;
using FeExcel.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace FeExcel.Web.Controllers;

/// <summary>Solo atiende la página de error genérica; la aplicación vive en <see cref="ImportacionController"/>.</summary>
public class HomeController : Controller
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
