using Microsoft.AspNetCore.Mvc;

namespace ec.edu.monster.controlador
{
  /// <summary>
  /// Indica si el servidor RESTful responde. La cabecera lo consulta cada pocos
  /// segundos para actualizar el indicador sin recargar la página.
  /// </summary>
  public class EstadoController(IFabricaServicios fabrica) : Controller
  {
    [HttpGet("/estado")]
    [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
    public async Task<IActionResult> Consultar()
    {
      return Json(new { disponible = await fabrica.DisponibleAsync() });
    }
  }
}
