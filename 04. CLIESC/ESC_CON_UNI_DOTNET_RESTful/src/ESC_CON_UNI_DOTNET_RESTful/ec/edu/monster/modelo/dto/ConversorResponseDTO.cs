using System.Text.Json;
using ec.edu.monster.modelo.enums;

namespace ec.edu.monster.modelo.dto
{
  /// <summary>
  /// Respuesta de POST /converter.
  /// </summary>
  public class ConversorResponseDTO : ResponseDTO
  {
    public double Resultado { get; set; }

    public ConversorResponseDTO() { }

    public ConversorResponseDTO(EstadosEnum estado, string? mensaje, double resultado) : base(estado, mensaje)
    {
      Resultado = resultado;
    }

    /// <exception cref="JsonException">Si el texto no es un objeto JSON.</exception>
    public static ConversorResponseDTO FromJSON(string texto)
    {
      using var documento = Leer(texto);
      var json = documento.RootElement;

      double resultado = json.TryGetProperty("resultado", out JsonElement valor)
        && valor.ValueKind == JsonValueKind.Number
          ? valor.GetDouble()
          : 0;

      return new ConversorResponseDTO(LeerEstado(json), LeerTexto(json, "mensaje"), resultado);
    }
  }
}
