using Microsoft.AspNetCore.Mvc;

namespace ec.edu.monster.controlador
{
  /// <summary>
  /// Menú principal que se muestra tras el login, con accesos al catálogo y a la conversión.
  /// </summary>
  [AutenticacionFilter]
  public class MenuController : Controller
  {
    [HttpGet("/menu")]
    public IActionResult Mostrar()
    {
      return View("menu");
    }
  }
}
