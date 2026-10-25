using ec.edu.monster.modelo.enums;

namespace ec.edu.monster.modelo.dto
{
  /// <summary>
  /// Respuesta de POST /login.
  /// </summary>
  public class LoginResponseDTO : ResponseDTO
  {
    public string? Token { get; set; }

    public LoginResponseDTO() { }

    public LoginResponseDTO(EstadosEnum estado, string? mensaje, string? token) : base(estado, mensaje)
    {
      Token = token;
    }

    /// <exception cref="System.Text.Json.JsonException">Si el texto no es un objeto JSON.</exception>
    public static LoginResponseDTO FromJSON(string texto)
    {
      using var documento = Leer(texto);
      var json = documento.RootElement;
      return new LoginResponseDTO(LeerEstado(json), LeerTexto(json, "mensaje"), LeerTexto(json, "token"));
    }
  }
}
