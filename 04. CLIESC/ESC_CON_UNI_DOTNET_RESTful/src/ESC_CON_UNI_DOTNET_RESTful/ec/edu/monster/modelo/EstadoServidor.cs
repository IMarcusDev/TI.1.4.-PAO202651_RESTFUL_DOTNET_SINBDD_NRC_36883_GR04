using System.Net;

namespace ec.edu.monster.modelo
{
  public interface IEstadoServidor
  {
    /// <summary>True cuando el servidor responde en este momento.</summary>
    Task<bool> DisponibleAsync();
  }

  /// <summary>
  /// Pregunta al servidor RESTful si está arriba. Se consulta cada vez, porque el
  /// servidor puede caerse en cualquier momento.
  /// </summary>
  public class EstadoServidor : IEstadoServidor
  {
    // Sin token el servidor vivo responde 401 con su JSON: basta para saber que está arriba.
    private const string RecursoDisponibilidad = "catalog/longitud";

    private readonly Uri disponibilidad;
    private readonly HttpClient http;

    public EstadoServidor(string urlBase, HttpMessageHandler? manejador = null)
    {
      disponibilidad = new Uri(urlBase.TrimEnd('/') + "/" + RecursoDisponibilidad);
      http = manejador == null ? new HttpClient() : new HttpClient(manejador);
      http.Timeout = TimeSpan.FromSeconds(2);
    }

    public async Task<bool> DisponibleAsync()
    {
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
