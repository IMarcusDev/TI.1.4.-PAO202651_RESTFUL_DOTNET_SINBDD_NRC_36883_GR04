using WS_CON_UNI_DOTNET_SOAP.Models.Enums;

namespace WS_CON_UNI_DOTNET_SOAP.Models.DTO
{
  public class LoginResponseDTO : ResponseDTO
  {
    public string? token { get; set; }

    public LoginResponseDTO() { }

    public LoginResponseDTO(EstadosEnum estado, string mensaje, string token) : base(estado, mensaje)
    {
      this.token = token;
    }
  }
}
