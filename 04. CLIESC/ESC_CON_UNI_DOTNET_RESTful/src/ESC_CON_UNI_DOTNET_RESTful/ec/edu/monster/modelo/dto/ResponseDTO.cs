using System.Text.Json;
using ec.edu.monster.modelo.enums;

namespace ec.edu.monster.modelo.dto
{
  /// <summary>
  /// Envoltura común de toda respuesta del servidor: un estado y un mensaje.
  /// </summary>
  public class ResponseDTO
  {
    public EstadosEnum Estado { get; set; }
    public string? Mensaje { get; set; }

    public ResponseDTO() { }

    public ResponseDTO(EstadosEnum estado, string? mensaje)
    {
      Estado = estado;
      Mensaje = mensaje;
    }

    /// <summary>
    /// Lee el texto como un objeto JSON.
    /// </summary>
    /// <exception cref="JsonException">Si el texto no es un objeto JSON, por ejemplo una página HTML de error.</exception>
    protected static JsonDocument Leer(string texto)
    {
      JsonDocument documento = JsonDocument.Parse(texto);
      if (documento.RootElement.ValueKind != JsonValueKind.Object)
      {
        documento.Dispose();
        throw new JsonException("La respuesta no es un objeto JSON");
      }

      return documento;
    }

    /// <summary>
    /// Lee el estado, que ASP.NET envía como número (0 = OK, 1 = ERROR) o como texto.
    /// Un estado ausente o desconocido se trata como error.
    /// </summary>
    protected static EstadosEnum LeerEstado(JsonElement json)
    {
      if (!json.TryGetProperty("estado", out JsonElement estado))
        return EstadosEnum.ERROR;

      if (estado.ValueKind == JsonValueKind.Number && estado.TryGetInt32(out int numero))
        return Enum.IsDefined(typeof(EstadosEnum), numero) ? (EstadosEnum)numero : EstadosEnum.ERROR;

      if (estado.ValueKind == JsonValueKind.String
          && Enum.TryParse(estado.GetString(), false, out EstadosEnum nombre)
          && Enum.IsDefined(nombre))
        return nombre;

      return EstadosEnum.ERROR;
    }

    protected static string? LeerTexto(JsonElement json, string propiedad)
    {
      return json.TryGetProperty(propiedad, out JsonElement valor) && valor.ValueKind == JsonValueKind.String
        ? valor.GetString()
        : null;
    }
  }
}
