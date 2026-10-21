using System.Text.Json;
using ec.edu.monster.modelo.dto;
using ec.edu.monster.modelo.enums;

namespace ec.edu.monster.modelo.http
{
  /// <summary>
  /// <see cref="ILoginApi"/> sobre HTTP: POST /login con cuerpo JSON y sin token.
  /// </summary>
  public class LoginRestApi(RESTfulClient cliente) : ILoginApi
  {
    public async Task<LoginResponseDTO> LoginAsync(string usuario, string clave)
    {
      string cuerpo = JsonSerializer.Serialize(new { username = usuario, password = clave });
      return LoginResponseDTO.FromJSON(await cliente.PostAsync(EndpointsEnum.login, cuerpo, null));
    }
  }
}
