using Microsoft.AspNetCore.Mvc;

namespace ec.edu.monster.controlador
{
  /// <summary>
  /// Cierra la sesión del usuario y vuelve a la página de login.
  /// </summary>
  [AutenticacionFilter]
  public class LogoutController : Controller
  {
    // Cerrar sesión cambia el estado, así que solo se hace con POST.
    [HttpGet("/logout")]
    public IActionResult Mostrar()
    {
      return Redirect("~/menu");
    }

    [HttpPost("/logout")]
    public IActionResult Salir()
    {
      SesionUsuario.Cerrar(HttpContext);
      return Redirect("~/login");
    }
  }
}
