using ec.edu.monster.modelo.http;

namespace ec.edu.monster.modelo
{
  /// <summary>
  /// Conecta el modelo con el servidor RESTful real. El cliente HTTP se crea una
  /// sola vez y lo comparten el login, el catálogo y la conversión.
  /// </summary>
  public static class ClienteRest
  {
    /// <summary>Crea el servicio de la aplicación, ligado a su sesión.</summary>
    public static ConversorServicio CrearServicio(string urlBase, Sesion sesion, HttpMessageHandler? manejador = null)
    {
      var restful = new RESTfulClient(urlBase, manejador);
      return new ConversorServicio(new LoginRestApi(restful), new ConversorRestApi(restful), sesion);
    }
  }
}
