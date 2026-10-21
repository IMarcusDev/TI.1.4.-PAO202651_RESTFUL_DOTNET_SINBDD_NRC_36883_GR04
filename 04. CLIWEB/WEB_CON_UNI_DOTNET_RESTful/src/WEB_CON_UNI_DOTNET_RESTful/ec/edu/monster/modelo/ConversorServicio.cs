using System.Text.Json;
using ec.edu.monster.modelo.dto;
using ec.edu.monster.modelo.enums;
using ec.edu.monster.modelo.http;

namespace ec.edu.monster.modelo
{
  /// <summary>
  /// Único punto de acceso al servidor RESTful. Los controladores usan esta clase,
  /// nunca la capa HTTP.
  /// </summary>
  public class ConversorServicio
  {
    public const string SinConexion = "No se pudo conectar con el servidor";
    public const string SinSesion = "La sesión no está activa";
    public const string SinRespuesta = "El servidor no devolvió una respuesta";
    public const string SinToken = "El servidor no devolvió un token de sesión";
    public const string ErrorGenerico = "El servidor respondió con un error";

    private readonly ILoginApi login;
    private readonly IConversorApi conversor;
    private readonly Sesion sesion;

    public ConversorServicio(ILoginApi login, IConversorApi conversor, Sesion sesion)
    {
      this.login = login;
      this.conversor = conversor;
      this.sesion = sesion;
    }

    public async Task IniciarSesionAsync(string usuario, string clave)
    {
      // Un nuevo intento de ingreso reemplaza la sesión anterior, aunque falle.
      sesion.Cerrar();

      LoginResponseDTO? respuesta = await LlamarAsync(() => login.LoginAsync(usuario, clave));
      Validar(respuesta);

      if (string.IsNullOrWhiteSpace(respuesta!.Token))
        throw new ServicioException(SinToken);

      sesion.Iniciar(usuario, respuesta.Token);
    }

    public async Task<double> ConvertirAsync(string magnitud, double valor, string desde, string hacia)
    {
      string token = TokenActivo();

      ConversorResponseDTO? respuesta = await LlamarAsync(() =>
        conversor.ConvertAsync(token, magnitud, valor, desde, hacia));
      Validar(respuesta);

      return respuesta!.Resultado;
    }

    public async Task<IReadOnlyList<string>> CatalogoAsync(string magnitud)
    {
      string token = TokenActivo();

      CatalogResponseDTO? respuesta = await LlamarAsync(() => conversor.CatalogAsync(token, magnitud));
      Validar(respuesta);

      return respuesta!.Resultado?.ToList() ?? [];
    }

    public void CerrarSesion()
    {
      sesion.Cerrar();
    }

    private string TokenActivo()
    {
      if (!sesion.EstaActiva())
        throw new ServicioException(SinSesion);

      return sesion.Token!;
    }

    private static void Validar(ResponseDTO? respuesta)
    {
      if (respuesta == null)
        throw new ServicioException(SinRespuesta);
      if (respuesta.Estado != EstadosEnum.OK)
        throw new ServicioException(MensajeOGenerico(respuesta.Mensaje));
    }

    private static async Task<T> LlamarAsync<T>(Func<Task<T>> llamada)
    {
      try
      {
        return await llamada();
      }
      catch (JsonException e)
      {
        // El servidor respondió, pero no con el JSON esperado (por ejemplo, una página HTML de error).
        throw new ServicioException(SinRespuesta, e);
      }
      catch (HttpRequestException e)
      {
        throw new ServicioException(SinConexion, e);
      }
      catch (TaskCanceledException e)
      {
        // HttpClient informa así que la petición excedió su tiempo.
        throw new ServicioException(SinConexion, e);
      }
    }

    private static string MensajeOGenerico(string? mensaje)
    {
      return string.IsNullOrWhiteSpace(mensaje) ? ErrorGenerico : mensaje;
    }
  }
}
