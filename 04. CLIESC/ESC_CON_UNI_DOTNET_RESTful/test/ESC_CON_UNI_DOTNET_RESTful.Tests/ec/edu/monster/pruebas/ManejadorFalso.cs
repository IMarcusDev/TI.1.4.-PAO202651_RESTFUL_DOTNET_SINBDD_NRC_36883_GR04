using System.Net;
using System.Text;

namespace ec.edu.monster.pruebas
{
  /// <summary>
  /// Transporte HTTP en memoria: guarda la última petición recibida y responde lo que se le indique.
  /// </summary>
  public class ManejadorFalso : HttpMessageHandler
  {
    public HttpStatusCode Estado { get; set; } = HttpStatusCode.OK;
    public string Cuerpo { get; set; } = "{}";
    public Exception? Falla { get; set; }

    public HttpMethod? Metodo { get; private set; }
    public Uri? Uri { get; private set; }
    public HttpRequestMessage? Peticion { get; private set; }
    public string? CuerpoEnviado { get; private set; }
    public string? TipoContenido { get; private set; }

    protected override async Task<HttpResponseMessage> SendAsync(
      HttpRequestMessage peticion, CancellationToken cancelacion)
    {
      Peticion = peticion;
      Metodo = peticion.Method;
      Uri = peticion.RequestUri;
      CuerpoEnviado = peticion.Content == null ? null : await peticion.Content.ReadAsStringAsync(cancelacion);
      TipoContenido = peticion.Content?.Headers.ContentType?.MediaType;

      if (Falla != null)
        throw Falla;

      return new HttpResponseMessage(Estado)
      {
        Content = new StringContent(Cuerpo, Encoding.UTF8, "application/json"),
      };
    }
  }
}
