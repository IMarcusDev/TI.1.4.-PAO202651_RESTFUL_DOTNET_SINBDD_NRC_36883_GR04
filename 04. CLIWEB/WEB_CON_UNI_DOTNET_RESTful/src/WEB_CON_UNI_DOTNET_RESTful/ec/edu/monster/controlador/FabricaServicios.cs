using System.Net;
using ec.edu.monster.modelo;
using ec.edu.monster.modelo.http;

namespace ec.edu.monster.controlador
{
  public interface IFabricaServicios
  {
    /// <summary>Crea el servicio de un usuario, ligado a su sesión.</summary>
    ConversorServicio CrearServicio(Sesion sesion);

    /// <summary>True cuando el servidor responde en este momento.</summary>
    Task<bool> DisponibleAsync();
  }

  /// <summary>
  /// Fábrica única de la aplicación. El cliente HTTP es seguro entre hilos, así que se
  /// crea una sola vez y se comparte; cada usuario recibe su propio ConversorServicio
  /// ligado a su propia Sesion.
  /// </summary>
  public class FabricaServicios : IFabricaServicios
  {
    // Sin token el servidor vivo responde 401 con su JSON: basta para saber que está arriba.
    private const string RecursoDisponibilidad = "catalog/longitud";
    private static readonly TimeSpan TiempoDisponible = TimeSpan.FromSeconds(2);

    private readonly RESTfulClient restful;
    private readonly Uri disponibilidad;
    private readonly HttpClient http;

    public FabricaServicios(string urlBase, HttpMessageHandler? manejador = null)
    {
      urlBase = urlBase.EndsWith('/') ? urlBase : urlBase + "/";

      restful = new RESTfulClient(urlBase, manejador);
      disponibilidad = new Uri(urlBase + RecursoDisponibilidad);
      http = manejador == null ? new HttpClient() : new HttpClient(manejador);
      http.Timeout = TiempoDisponible;
    }

    public ConversorServicio CrearServicio(Sesion sesion)
    {
      return new ConversorServicio(new LoginRestApi(restful), new ConversorRestApi(restful), sesion);
    }

    public async Task<bool> DisponibleAsync()
    {
      // Se pregunta cada vez: el servidor puede caerse en cualquier momento.
      try
      {
        using var respuesta = await http.GetAsync(disponibilidad, HttpCompletionOption.ResponseHeadersRead);
        return respuesta.StatusCode is HttpStatusCode.OK or HttpStatusCode.Unauthorized;
      }
      catch (HttpRequestException)
      {
        return false;
      }
      catch (TaskCanceledException)
      {
        return false;
      }
    }
  }
}
