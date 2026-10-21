using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ec.edu.monster.controlador
{
  /// <summary>
  /// Las páginas protegidas exigen una sesión activa; si no la hay, el usuario
  /// vuelve a la página de login. También indica a la cabecera si el servidor responde.
  /// </summary>
  public class AutenticacionFilter : ActionFilterAttribute
  {
    public override async Task OnActionExecutionAsync(ActionExecutingContext contexto, ActionExecutionDelegate siguiente)
    {
      if (!SesionUsuario.Activa(contexto.HttpContext))
      {
        contexto.Result = new RedirectResult("~/login");
        return;
      }

      // Las páginas protegidas no deben verse desde la caché del navegador tras cerrar sesión.
      contexto.HttpContext.Response.Headers.CacheControl = "no-store";

      if (contexto.Controller is Controller controlador)
      {
        var fabrica = contexto.HttpContext.RequestServices.GetRequiredService<IFabricaServicios>();
        controlador.ViewData["ServicioDisponible"] = await fabrica.DisponibleAsync();
      }

      await siguiente();
    }
  }
}
