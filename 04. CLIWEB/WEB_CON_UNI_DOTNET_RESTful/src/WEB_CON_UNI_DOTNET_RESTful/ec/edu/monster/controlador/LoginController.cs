using ec.edu.monster.modelo;
using Microsoft.AspNetCore.Mvc;

namespace ec.edu.monster.controlador
{
  /// <summary>
  /// Página de login: valida los datos, pide al modelo iniciar sesión y, si todo
  /// sale bien, guarda la sesión del usuario.
  /// </summary>
  public class LoginController(IFabricaServicios fabrica) : Controller
  {
    public const string CredencialesVacias = "Ingrese usuario y contraseña";

    [HttpGet("/")]
    public IActionResult Inicio()
    {
      return Redirect("~/login");
    }

    [HttpGet("/login")]
    public async Task<IActionResult> Mostrar()
    {
      if (SesionUsuario.Activa(HttpContext))
        return Redirect("~/menu");

      return await VistaAsync(new LoginVista());
    }

    [HttpPost("/login")]
    public async Task<IActionResult> Ingresar(string? usuario, string? clave)
    {
      // La contraseña nunca se devuelve a la página.
      var vista = new LoginVista { Usuario = usuario ?? "" };

      if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(clave))
      {
        vista.Error = CredencialesVacias;
        return await VistaAsync(vista);
      }

      var sesion = new Sesion();
      try
      {
        await fabrica.CrearServicio(sesion).IniciarSesionAsync(usuario.Trim(), clave);
      }
      catch (ServicioException e)
      {
        SesionUsuario.Cerrar(HttpContext);
        vista.Error = e.Message;
        return await VistaAsync(vista);
      }

      SesionUsuario.Guardar(HttpContext, sesion);
      return Redirect("~/menu");
    }

    private async Task<IActionResult> VistaAsync(LoginVista vista)
    {
      ViewData["ServicioDisponible"] = await fabrica.DisponibleAsync();
      return View("login", vista);
    }
  }
}
