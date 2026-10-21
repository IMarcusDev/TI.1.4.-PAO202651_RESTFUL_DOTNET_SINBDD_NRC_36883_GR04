using System.Net;
using ec.edu.monster.modelo.enums;
using ec.edu.monster.modelo.http;

namespace ec.edu.monster.pruebas
{
  public class RESTfulClientTest
  {
    private readonly ManejadorFalso manejador = new();

    private RESTfulClient Cliente(string urlBase = "http://servidor:5013") => new(urlBase, manejador);

    [Fact]
    public async Task GetEnviaElTokenEnLaCabeceraAuthorization()
    {
      await Cliente().GetAsync("catalog/Longitud", "abc-123");

      Assert.Equal(HttpMethod.Get, manejador.Metodo);
      Assert.Equal("http://servidor:5013/catalog/Longitud", manejador.Uri!.ToString());
      Assert.Equal("abc-123", manejador.Peticion!.Headers.GetValues("Authorization").Single());
      Assert.Equal("application/json", manejador.Peticion.Headers.Accept.Single().MediaType);
    }

    [Fact]
    public async Task PostEnviaJsonYElTokenEnLaCabeceraAuthorization()
    {
      await Cliente().PostAsync(EndpointsEnum.converter, "{\"type\":\"Masa\"}", "abc-123");

      Assert.Equal(HttpMethod.Post, manejador.Metodo);
      Assert.Equal("http://servidor:5013/converter", manejador.Uri!.ToString());
      Assert.Equal("application/json", manejador.TipoContenido);
      Assert.Equal("{\"type\":\"Masa\"}", manejador.CuerpoEnviado);
      Assert.Equal("abc-123", manejador.Peticion!.Headers.GetValues("Authorization").Single());
    }

    [Fact]
    public async Task SinTokenNoSeEnviaLaCabeceraAuthorization()
    {
      await Cliente().PostAsync(EndpointsEnum.login, "{}", null);

      Assert.False(manejador.Peticion!.Headers.Contains("Authorization"));
    }

    [Fact]
    public async Task LaUrlBaseConBarraFinalNoDuplicaLaBarra()
    {
      await Cliente("http://servidor:5013/").PostAsync(EndpointsEnum.login, "{}", null);

      Assert.Equal("http://servidor:5013/login", manejador.Uri!.ToString());
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized)]
    [InlineData(HttpStatusCode.BadRequest)]
    public async Task DevuelveElCuerpoAunqueElEstadoSeaDeError(HttpStatusCode estado)
    {
      manejador.Estado = estado;
      manejador.Cuerpo = "{\"estado\":1,\"mensaje\":\"Token Inválido\"}";

      string cuerpo = await Cliente().GetAsync("catalog/Masa", "mal");

      Assert.Equal("{\"estado\":1,\"mensaje\":\"Token Inválido\"}", cuerpo);
    }

    [Fact]
    public async Task UnaCaidaDeRedSePropagaComoHttpRequestException()
    {
      manejador.Falla = new HttpRequestException("sin red");

      await Assert.ThrowsAsync<HttpRequestException>(() => Cliente().GetAsync("catalog/Masa", "t"));
    }
  }
}
