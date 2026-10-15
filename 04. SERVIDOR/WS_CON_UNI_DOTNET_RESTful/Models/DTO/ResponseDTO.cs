using WS_CON_UNI_DOTNET_RESTful.Models.Enums;

namespace WS_CON_UNI_DOTNET_RESTful.Models.DTO
{
  public class ResponseDTO
  {
    public EstadosEnum estado { get; set; }
    public string? mensaje { get; set; }

    public ResponseDTO() { }

    public ResponseDTO(EstadosEnum estado, string mensaje)
    {
      this.estado = estado;
      this.mensaje = mensaje;
    }
  }
}
