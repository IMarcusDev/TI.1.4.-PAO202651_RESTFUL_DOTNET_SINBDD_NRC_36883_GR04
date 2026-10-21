using System.Text.Json;
using ec.edu.monster.modelo.dto;
using ec.edu.monster.modelo.enums;

namespace ec.edu.monster.pruebas
{
  public class ResponseDTOTest
  {
    [Theory]
    [InlineData("{\"estado\":0}", EstadosEnum.OK)]
    [InlineData("{\"estado\":1}", EstadosEnum.ERROR)]
    [InlineData("{\"estado\":\"OK\"}", EstadosEnum.OK)]
    [InlineData("{\"estado\":\"ERROR\"}", EstadosEnum.ERROR)]
    public void LeeElEstadoComoNumeroOComoTexto(string json, EstadosEnum esperado)
    {
      Assert.Equal(esperado, LoginResponseDTO.FromJSON(json).Estado);
    }

    [Theory]
    [InlineData("{}")]
    [InlineData("{\"estado\":7}")]
    [InlineData("{\"estado\":\"otro\"}")]
    [InlineData("{\"estado\":null}")]
    public void UnEstadoAusenteODesconocidoEsError(string json)
    {
      Assert.Equal(EstadosEnum.ERROR, ConversorResponseDTO.FromJSON(json).Estado);
    }

    [Fact]
    public void LeeElMensajeYUnMensajeNuloQuedaNulo()
    {
      Assert.Equal("Hola", LoginResponseDTO.FromJSON("{\"estado\":1,\"mensaje\":\"Hola\"}").Mensaje);
      Assert.Null(LoginResponseDTO.FromJSON("{\"estado\":1,\"mensaje\":null}").Mensaje);
      Assert.Null(LoginResponseDTO.FromJSON("{\"estado\":1}").Mensaje);
    }

    [Fact]
    public void UnTokenNuloOAusenteQuedaNulo()
    {
      Assert.Null(LoginResponseDTO.FromJSON("{\"estado\":1,\"token\":null}").Token);
      Assert.Null(LoginResponseDTO.FromJSON("{\"estado\":1}").Token);
    }

    [Fact]
    public void UnResultadoAusenteEnLaConversionEsCero()
    {
      Assert.Equal(0, ConversorResponseDTO.FromJSON("{\"estado\":1}").Resultado);
    }

    [Fact]
    public void UnCatalogoSinResultadoQuedaNulo()
    {
      Assert.Null(CatalogResponseDTO.FromJSON("{\"estado\":1,\"resultado\":null}").Resultado);
      Assert.Null(CatalogResponseDTO.FromJSON("{\"estado\":1}").Resultado);
    }

    [Theory]
    [InlineData("<html>500</html>")]
    [InlineData("")]
    [InlineData("[1,2]")]
    public void UnTextoQueNoEsUnObjetoJsonLanzaJsonException(string texto)
    {
      Assert.ThrowsAny<JsonException>(() => LoginResponseDTO.FromJSON(texto));
    }
  }
}
