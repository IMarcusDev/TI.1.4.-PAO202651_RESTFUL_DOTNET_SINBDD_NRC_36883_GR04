using WS_CON_UNI_DOTNET_RESTful.Models.Enums;

namespace WS_CON_UNI_DOTNET_RESTful.Models.DTO
{
  public class ConverterResponseDTO : ResponseDTO
  {
    public double resultado { get; set; }

    public ConverterResponseDTO() { }

    public ConverterResponseDTO(EstadosEnum estado, String mensaje, double resultado) : base(estado, mensaje)
    {
      this.resultado = resultado;
    }
  }
}
