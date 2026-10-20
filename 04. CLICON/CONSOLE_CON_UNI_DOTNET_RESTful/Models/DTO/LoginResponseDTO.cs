using CONSOLE_CON_UNI_DOTNET_RESTful.Models.Enums;
using System.Text.Json;

namespace CONSOLE_CON_UNI_DOTNET_RESTful.Models.DTO
{
  internal class LoginResponseDTO : ResponseDTO
  {
    public string token;

    public LoginResponseDTO(
      EstadosEnum estado,
      string mensaje,
      string token
    ) : base(estado, mensaje)
    {
      this.token = token;
    }

    public string GetToken()
    {
      return token;
    }

    public void SetToken(string token)
    {
      this.token = token;
    }

    public static new LoginResponseDTO FromJSON(string json)
    {
      ResponseDTO response = ResponseDTO.FromJSON(json);

      using JsonDocument doc = JsonDocument.Parse(json);
      JsonElement obj = doc.RootElement;

      string token = obj.TryGetProperty("token", out JsonElement tokenElement)
        ? tokenElement.GetString() ?? ""
        : "";

      return new LoginResponseDTO(
        response.GetEstado(),
        response.GetMensaje(),
        token
      );
    }
  }
}
