using System.Text.Json;
using ec.edu.monster.controlador;
using ec.edu.monster.modelo;
using ec.edu.monster.modelo.dto;
using ec.edu.monster.modelo.enums;
using ec.edu.monster.modelo.http;

namespace ec.edu.monster.pruebas
{
  /// <summary>
  /// Servidor RESTful en memoria con las mismas reglas que el real: usuario "user",
  /// clave "1234" y token obligatorio. Puede simular caídas y respuestas inválidas.
  /// </summary>
  public class ServidorFalso : ILoginApi, IConversorApi
  {
    public const string Token = "token-prueba";

    public bool Caido { get; set; }
    public bool RespuestaNoJson { get; set; }
    public bool SinRespuesta { get; set; }
    public (string Token, string Tipo, double Valor, string Desde, string Hacia)? UltimaConversion { get; private set; }

    private static readonly Dictionary<string, string[]> Catalogo = new()
    {
      ["Longitud"] = ["m", "cm", "ft"],
      ["Masa"] = ["g", "kg"],
      ["Volumen"] = ["mL", "L"],
      ["Tiempo"] = ["s", "min"],
      ["Temperatura"] = ["°C", "°F"],
      ["Area"] = ["m2", "ha"],
    };

    public Task<LoginResponseDTO> LoginAsync(string usuario, string clave)
    {
      Simular();
      LoginResponseDTO dto = usuario == "user" && clave == "1234"
        ? new LoginResponseDTO(EstadosEnum.OK, "Ingreso de Sesión Exitoso!", Token)
        : new LoginResponseDTO(EstadosEnum.ERROR, "Credenciales Incorrectas", "");

      return Task.FromResult(SinRespuesta ? null! : dto);
    }

    public Task<CatalogResponseDTO> CatalogAsync(string token, string type)
    {
      Simular();
      CatalogResponseDTO dto;

      if (token != Token)
        dto = new CatalogResponseDTO(EstadosEnum.ERROR, "Token Inválido", []);
      else if (Catalogo.TryGetValue(type, out var unidades))
        dto = new CatalogResponseDTO(EstadosEnum.OK, "Obtención de Catálogo Exitosa", [.. unidades]);
      else
        dto = new CatalogResponseDTO(EstadosEnum.ERROR, "Tipo de Catálogo Inexistente", []);

      return Task.FromResult(SinRespuesta ? null! : dto);
    }

    public Task<ConversorResponseDTO> ConvertAsync(string token, string type, double value, string from, string to)
    {
      Simular();
      UltimaConversion = (token, type, value, from, to);

      ConversorResponseDTO dto = token == Token
        ? new ConversorResponseDTO(EstadosEnum.OK, "Conversión Exitosa", value * 100)
        : new ConversorResponseDTO(EstadosEnum.ERROR, "Token Inválido", 0);

      return Task.FromResult(SinRespuesta ? null! : dto);
    }

    private void Simular()
    {
      if (Caido)
        throw new HttpRequestException("Servidor caído");
      if (RespuestaNoJson)
        throw new JsonException("El servidor respondió una página HTML");
    }
  }

  /// <summary>Fábrica que conecta la aplicación con el servidor en memoria.</summary>
  public class FabricaFalsa(ServidorFalso servidor) : IFabricaServicios
  {
    public ConversorServicio CrearServicio(Sesion sesion) =>
      new(servidor, servidor, sesion);

    public Task<bool> DisponibleAsync() => Task.FromResult(!servidor.Caido);
  }
}
