using CONSOLE_CON_UNI_DOTNET_RESTful.Models.Enums;
using System.Text.Json;

namespace CONSOLE_CON_UNI_DOTNET_RESTful.Models.DTO
{
  internal class ConversorResponseDTO : ResponseDTO
  {
    private double resultado;

    public ConversorResponseDTO(
        EstadosEnum estado,
        string mensaje,
        double resultado)
        : base(estado, mensaje)
    {
      this.resultado = resultado;
    }

    public double GetResultado()
    {
      return resultado;
    }

    public void SetResultado(double resultado)
    {
      this.resultado = resultado;
    }

    public static new ConversorResponseDTO FromJSON(string json)
    {
      ResponseDTO response = ResponseDTO.FromJSON(json);

      using JsonDocument doc = JsonDocument.Parse(json);
      JsonElement obj = doc.RootElement;

      double resultado = obj.TryGetProperty(
          "resultado",
          out JsonElement resultadoElement
      ) && resultadoElement.ValueKind == JsonValueKind.Number
          ? resultadoElement.GetDouble()
          : 0;

      return new ConversorResponseDTO(
          response.GetEstado(),
          response.GetMensaje(),
          resultado
      );
    }
  }
}
