using ec.edu.monster.controlador;
using ec.edu.monster.modelo;

namespace ec.edu.monster.pruebas
{
  public class ConversionControladorTest
  {
    private readonly ServidorFalso servidor = new();
    private readonly Sesion sesion = new();
    private readonly VistaConversionFalsa vista = new();
    private readonly ConversionControlador controlador;

    public ConversionControladorTest()
    {
      sesion.Iniciar("user", ServidorFalso.Token);
      controlador = new ConversionControlador(vista, new ConversorServicio(servidor, servidor, sesion));
    }

    [Fact]
    public async Task AlIniciarMuestraLasSeisMagnitudesYLasUnidadesDeLaPrimera()
    {
      await controlador.IniciarAsync();

      Assert.Equal(["Longitud", "Masa", "Volumen", "Tiempo", "Temperatura", "Area"], vista.Magnitudes);
      Assert.Equal(["m", "cm", "ft"], vista.Unidades);
      Assert.Equal("m", vista.UnidadOrigen);
      Assert.Equal("cm", vista.UnidadDestino);
    }

    [Fact]
    public async Task AlCambiarLaMagnitudRecargaSusUnidades()
    {
      await controlador.IniciarAsync();

      await vista.SeleccionarMagnitud("Masa");

      Assert.Equal(["g", "kg"], vista.Unidades);
    }

    [Fact]
    public async Task ConvierteYMuestraElResultadoAceptandoComaDecimal()
    {
      await controlador.IniciarAsync();
      vista.Valor = "2,5";

      await vista.PresionarConvertir();

      Assert.Equal("2,5 m = 250,0000 cm", vista.Resultado);
      Assert.Null(vista.Error);
      Assert.Equal((ServidorFalso.Token, "Longitud", 2.5, "m", "cm"), servidor.UltimaConversion);
    }

    [Theory]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("1e3")]
    public async Task UnValorInvalidoNoLlamaAlServidor(string valor)
    {
      await controlador.IniciarAsync();
      vista.Valor = valor;

      await vista.PresionarConvertir();

      Assert.Equal(ValidadorConversion.ValorInvalido, vista.Error);
      Assert.Null(servidor.UltimaConversion);
    }

    [Fact]
    public async Task SinUnidadesNoSeConvierte()
    {
      await controlador.IniciarAsync();
      vista.Valor = "1";
      vista.UnidadDestino = null;

      await vista.PresionarConvertir();

      Assert.Equal(ValidadorConversion.SinUnidades, vista.Error);
      Assert.Null(servidor.UltimaConversion);
    }

    [Fact]
    public async Task UnNuevoIntentoBorraElResultadoAnterior()
    {
      await controlador.IniciarAsync();
      vista.Valor = "1";
      await vista.PresionarConvertir();
      Assert.NotNull(vista.Resultado);

      vista.Valor = "x";
      await vista.PresionarConvertir();

      Assert.Null(vista.Resultado);
      Assert.Equal(ValidadorConversion.ValorInvalido, vista.Error);
    }

    [Fact]
    public async Task SiElServidorSeCaeMuestraElErrorYNoDejaUnidadesViejas()
    {
      await controlador.IniciarAsync();
      servidor.Caido = true;

      await vista.SeleccionarMagnitud("Masa");

      Assert.Empty(vista.Unidades);
      Assert.Equal(ConversorServicio.SinConexion, vista.Error);
    }

    [Fact]
    public async Task UnTokenRechazadoMuestraElMensajeDelServidor()
    {
      await controlador.IniciarAsync();
      sesion.Iniciar("user", "vencido");
      vista.Valor = "1";

      await vista.PresionarConvertir();

      Assert.Equal("Token Inválido", vista.Error);
    }
  }
}
