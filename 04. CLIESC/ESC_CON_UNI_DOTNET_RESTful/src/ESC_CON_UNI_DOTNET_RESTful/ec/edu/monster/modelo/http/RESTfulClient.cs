using System.Text;
using ec.edu.monster.modelo.enums;

namespace ec.edu.monster.modelo.http
{
  /// <summary>
  /// Transporte HTTP mínimo hacia el servidor RESTful. El token de la sesión viaja en la
  /// cabecera Authorization y se omite cuando es null (login). El cuerpo se devuelve como
  /// texto sea cual sea el código de estado, porque el servidor informa sus errores dentro
  /// de un cuerpo JSON.
  /// </summary>
  public class RESTfulClient
  {
    public const string UrlBase = "http://localhost:5013/";

    // El servidor lo comparten varios clientes y puede trabarse: mejor fallar que esperar sin fin.
    private static readonly TimeSpan TiempoPeticion = TimeSpan.FromSeconds(15);

    private readonly string urlBase;
    private readonly HttpClient http;

    public RESTfulClient() : this(UrlBase) { }

    public RESTfulClient(string urlBase, HttpMessageHandler? manejador = null)
    {
      this.urlBase = urlBase.EndsWith('/') ? urlBase : urlBase + "/";
      http = manejador == null ? new HttpClient() : new HttpClient(manejador);
      http.Timeout = TiempoPeticion;
    }

    public Task<string> GetAsync(string recurso, string? token)
    {
      return EnviarAsync(Peticion(HttpMethod.Get, recurso, token));
    }

    public Task<string> PostAsync(EndpointsEnum endpoint, string json, string? token)
    {
      var peticion = Peticion(HttpMethod.Post, endpoint.ToString(), token);
      peticion.Content = new StringContent(json, Encoding.UTF8, "application/json");
      return EnviarAsync(peticion);
    }

    private HttpRequestMessage Peticion(HttpMethod metodo, string recurso, string? token)
    {
      var peticion = new HttpRequestMessage(metodo, urlBase + recurso);
      peticion.Headers.Accept.ParseAdd("application/json");
      if (token != null)
        peticion.Headers.TryAddWithoutValidation("Authorization", token);
      return peticion;
    }

    private async Task<string> EnviarAsync(HttpRequestMessage peticion)
    {
      using (peticion)
      {
        using var respuesta = await http.SendAsync(peticion);
        return await respuesta.Content.ReadAsStringAsync();
      }
    }
  }
}
