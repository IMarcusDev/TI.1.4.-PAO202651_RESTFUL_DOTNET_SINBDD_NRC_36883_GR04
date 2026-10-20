using CONSOLE_CON_UNI_DOTNET_RESTful.Models.Enums;
using System.Text.Json;

namespace CONSOLE_CON_UNI_DOTNET_RESTful.Models.DTO
{
  internal class ResponseDTO
  {
    public EstadosEnum estado;
    public string mensaje;

    public ResponseDTO(EstadosEnum estado, string mensaje)
    {
      this.estado = estado;
      this.mensaje = mensaje;
    }

    public EstadosEnum GetEstado()
    {
      return estado;
    }

    public void SetEstado(EstadosEnum estado)
    {
      this.estado = estado;
    }

    public string GetMensaje()
    {
      return mensaje;
    }

    public void SetMensaje(string mensaje)
    {
      this.mensaje = mensaje;
    }

    public static ResponseDTO FromJSON(string json)
    {
      using JsonDocument doc = JsonDocument.Parse(json);

      JsonElement obj = doc.RootElement;
      JsonElement estadoElement = obj.GetProperty("estado");

      EstadosEnum estado = (EstadosEnum)estadoElement.GetInt32();

      string mensaje = obj.GetProperty("mensaje").GetString()!;

      return new ResponseDTO(estado, mensaje);
    }
  }

}
