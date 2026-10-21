using ec.edu.monster.modelo;
using Microsoft.AspNetCore.Mvc;

namespace ec.edu.monster.controlador
{
  /// <summary>
  /// Página de conversión: carga las unidades de la magnitud elegida, valida los
  /// datos y muestra el resultado con formato. Se conservan las selecciones del usuario.
  /// </summary>
  [AutenticacionFilter]
  public class ConversionController(IFabricaServicios fabrica) : Controller
  {
    private const string CambiarMagnitud = "cambiar";

    [HttpGet("/conversion")]
    public async Task<IActionResult> Mostrar(string? magnitud)
    {
      var vista = new ConversionVista { Magnitud = MagnitudOPrimera(magnitud) };
      await CargarUnidadesAsync(vista);
      return Vista(vista);
    }

    [HttpPost("/conversion")]
    public async Task<IActionResult> Enviar(
      string? magnitud, string? desde, string? hacia, string? valor, string? accion)
    {
      var vista = new ConversionVista
      {
        Magnitud = magnitud,
        Desde = desde,
        Hacia = hacia,
        Valor = valor ?? "",
      };

      if (accion == CambiarMagnitud)
      {
        // Solo cambió la magnitud: se recargan sus unidades sin convertir.
        vista.Magnitud = MagnitudOPrimera(vista.Magnitud);
        await CargarUnidadesAsync(vista);
        return Vista(vista);
      }

      IReadOnlyList<string> unidades = await CargarUnidadesAsync(vista);
      if (vista.Error == null)
        await ConvertirAsync(vista, unidades);

      return Vista(vista);
    }

    private async Task ConvertirAsync(ConversionVista vista, IReadOnlyList<string> unidades)
    {
      string? error = ValidadorConversion.Validar(vista.Valor, vista.Magnitud, vista.Desde, vista.Hacia);

      // Unidades de otra magnitud (por ejemplo, un formulario viejo) cuentan como no elegidas.
      if (error == null && (!unidades.Contains(vista.Desde!) || !unidades.Contains(vista.Hacia!)))
        error = ValidadorConversion.SinUnidades;

      if (error != null)
      {
        vista.Error = error;
        return;
      }

      double valor = ValidadorConversion.LeerValor(vista.Valor)!.Value;
      try
      {
        var servicio = fabrica.CrearServicio(SesionUsuario.Leer(HttpContext));
        double resultado = await servicio.ConvertirAsync(vista.Magnitud!, valor, vista.Desde!, vista.Hacia!);
        vista.Resultado = FormatoResultado.Formatear(valor, vista.Desde!, resultado, vista.Hacia!);
      }
      catch (ServicioException e)
      {
        vista.Error = e.Message;
      }
    }

    // Carga las unidades de la magnitud del formulario; si falla, la página queda sin unidades y con el error.
    private async Task<IReadOnlyList<string>> CargarUnidadesAsync(ConversionVista vista)
    {
      IReadOnlyList<string> unidades = [];

      if (Magnitudes.Existe(vista.Magnitud))
      {
        try
        {
          var servicio = fabrica.CrearServicio(SesionUsuario.Leer(HttpContext));
          unidades = await servicio.CatalogoAsync(vista.Magnitud!);
        }
        catch (ServicioException e)
        {
          vista.Error = e.Message;
        }
      }

      if (vista.Desde == null || !unidades.Contains(vista.Desde))
        vista.Desde = null;
      if (vista.Hacia == null || !unidades.Contains(vista.Hacia))
        vista.Hacia = null;

      vista.Unidades = Opcion.Unidades(unidades);
      return unidades;
    }

    private IActionResult Vista(ConversionVista vista)
    {
      vista.Magnitudes = Opcion.Magnitudes();
      ViewData["Seccion"] = "conversion";
      return View("conversion", vista);
    }

    private static string MagnitudOPrimera(string? magnitud)
    {
      return Magnitudes.Existe(magnitud) ? magnitud! : Magnitudes.Todas[0];
    }
  }
}
