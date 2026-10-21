using ec.edu.monster.modelo;
using Microsoft.AspNetCore.Mvc;

namespace ec.edu.monster.controlador
{
  /// <summary>
  /// Página de catálogo: lista las unidades disponibles de la magnitud elegida.
  /// </summary>
  [AutenticacionFilter]
  public class CatalogoController(IFabricaServicios fabrica) : Controller
  {
    [HttpGet("/catalogo")]
    public async Task<IActionResult> Mostrar(string? magnitud)
    {
      // Una magnitud desconocida o vacía pasa a ser la primera.
      var vista = new CatalogoVista
      {
        Magnitudes = Opcion.Magnitudes(),
        Magnitud = Magnitudes.Existe(magnitud) ? magnitud! : Magnitudes.Todas[0],
      };

      try
      {
        var servicio = fabrica.CrearServicio(SesionUsuario.Leer(HttpContext));
        vista.Unidades = Opcion.Unidades(await servicio.CatalogoAsync(vista.Magnitud));
      }
      catch (ServicioException e)
      {
        // Nunca se muestran unidades de otra magnitud junto al error.
        vista.Error = e.Message;
      }

      ViewData["Seccion"] = "catalogo";
      return View("catalogo", vista);
    }
  }
}
