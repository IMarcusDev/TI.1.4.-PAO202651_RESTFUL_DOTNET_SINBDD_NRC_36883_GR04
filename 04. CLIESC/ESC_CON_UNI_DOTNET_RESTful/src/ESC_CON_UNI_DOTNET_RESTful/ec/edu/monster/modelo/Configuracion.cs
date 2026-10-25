using System.Text.Json;

namespace ec.edu.monster.modelo
{
  /// <summary>
  /// Lee la dirección del servidor RESTful de servidor.json, junto al ejecutable.
  /// Si el archivo falta o está mal escrito, se usa la dirección por defecto.
  /// </summary>
  public static class Configuracion
  {
    public const string UrlPorDefecto = "http://localhost:5013";
    public const string Archivo = "servidor.json";

    public static string LeerUrlServidor(string carpeta)
    {
      try
      {
        string texto = File.ReadAllText(Path.Combine(carpeta, Archivo));
        using var json = JsonDocument.Parse(texto);
        if (json.RootElement.TryGetProperty("UrlBase", out var url)
            && Uri.TryCreate(url.GetString(), UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
        {
          return uri.ToString().TrimEnd('/');
        }
      }
      catch (IOException) { }
      catch (UnauthorizedAccessException) { }
      catch (JsonException) { }
      catch (InvalidOperationException) { }

      return UrlPorDefecto;
    }
  }
}
