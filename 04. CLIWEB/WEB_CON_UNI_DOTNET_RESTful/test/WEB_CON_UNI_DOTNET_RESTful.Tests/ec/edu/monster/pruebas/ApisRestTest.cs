using System.Text.Json;
using ec.edu.monster.modelo.dto;
using ec.edu.monster.modelo.enums;
using ec.edu.monster.modelo.http;

namespace ec.edu.monster.pruebas
{
  public class ApisRestTest
  {
    private readonly ManejadorFalso manejador = new();

    private RESTfulClient Cliente() => new("http://servidor:5013", manejador);

    [Fact]
    public async Task ElLoginEnviaUsernameYPasswordSinToken()
    {
      manejador.Cuerpo = "{\"estado\":0,\"mensaje\":\"Ingreso de Sesión Exitoso!\",\"token\":\"t-1\"}";

      LoginResponseDTO respuesta = await new LoginRestApi(Cliente()).LoginAsync("user", "1234");

      Assert.Equal("http://servidor:5013/login", manejador.Uri!.ToString());
      Assert.Equal("{\"username\":\"user\",\"password\":\"1234\"}", manejador.CuerpoEnviado);
      Assert.False(manejador.Peticion!.Headers.Contains("Authorization"));
      Assert.Equal(EstadosEnum.OK, respuesta.Estado);
      Assert.Equal("t-1", respuesta.Token);
    }

    [Fact]
    public async Task ConvertirEnviaElValorComoNumeroYElTokenEnLaCabecera()
    {
      manejador.Cuerpo = "{\"estado\":0,\"mensaje\":\"Conversión Exitosa\",\"resultado\":250.5}";

      ConversorResponseDTO respuesta =
        await new ConversorRestApi(Cliente()).ConvertAsync("t-1", "Longitud", 2.5, "m", "cm");

      Assert.Equal("http://servidor:5013/converter", manejador.Uri!.ToString());
      Assert.Equal("t-1", manejador.Peticion!.Headers.GetValues("Authorization").Single());
      using var cuerpo = JsonDocument.Parse(manejador.CuerpoEnviado!);
      Assert.Equal("Longitud", cuerpo.RootElement.GetProperty("type").GetString());
      Assert.Equal(JsonValueKind.Number, cuerpo.RootElement.GetProperty("value").ValueKind);
      Assert.Equal(2.5, cuerpo.RootElement.GetProperty("value").GetDouble());
      Assert.Equal("m", cuerpo.RootElement.GetProperty("from").GetString());
      Assert.Equal("cm", cuerpo.RootElement.GetProperty("to").GetString());
      Assert.Equal(250.5, respuesta.Resultado);
    }

    [Fact]
    public async Task ElCatalogoPideElRecursoDeLaMagnitudYLeeLasUnidadesEnOrden()
    {
      manejador.Cuerpo = "{\"estado\":0,\"mensaje\":\"ok\",\"resultado\":[\"m\",\"cm\",\"ft\"]}";

      CatalogResponseDTO respuesta = await new ConversorRestApi(Cliente()).CatalogAsync("t-1", "Longitud");

      Assert.Equal(HttpMethod.Get, manejador.Metodo);
      Assert.Equal("http://servidor:5013/catalog/Longitud", manejador.Uri!.ToString());
      Assert.Equal("t-1", manejador.Peticion!.Headers.GetValues("Authorization").Single());
      Assert.Equal(["m", "cm", "ft"], respuesta.Resultado);
    }

    [Fact]
    public async Task ElNombreDeLaMagnitudSeCodificaEnLaUrl()
    {
      await new ConversorRestApi(Cliente()).CatalogAsync("t-1", "Área/Extra");

      Assert.Equal("/catalog/%C3%81rea%2FExtra", manejador.Uri!.AbsolutePath);
    }
  }
}
