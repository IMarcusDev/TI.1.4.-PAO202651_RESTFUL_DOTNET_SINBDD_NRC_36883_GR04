using ec.edu.monster.controlador;
using ec.edu.monster.modelo;

namespace ec.edu.monster.pruebas
{
  public class CatalogoControladorTest
  {
    private readonly ServidorFalso servidor = new();
    private readonly Sesion sesion = new();
    private readonly VistaCatalogoFalsa vista = new();
    private readonly CatalogoControlador controlador;

    public CatalogoControladorTest()
    {
      sesion.Iniciar("user", ServidorFalso.Token);
      controlador = new CatalogoControlador(vista, new ConversorServicio(servidor, servidor, sesion));
    }

    [Fact]
    public async Task AlIniciarListaLasUnidadesDeLaPrimeraMagnitud()
    {
      await controlador.IniciarAsync();

      Assert.Equal(6, vista.Magnitudes.Count);
      Assert.Equal(["m", "cm", "ft"], vista.Unidades);
    }

    [Fact]
    public async Task AlElegirOtraMagnitudMuestraSusUnidades()
    {
      await controlador.IniciarAsync();

      await vista.SeleccionarMagnitud("Temperatura");

      Assert.Equal(["°C", "°F"], vista.Unidades);
      Assert.Null(vista.Error);
    }

    [Fact]
    public async Task SiFallaNoQuedanLasUnidadesAnteriores()
    {
      await controlador.IniciarAsync();
      servidor.RespuestaNoJson = true;

      await vista.SeleccionarMagnitud("Masa");

      Assert.Empty(vista.Unidades);
      Assert.Equal(ConversorServicio.SinRespuesta, vista.Error);
    }

    [Fact]
    public async Task ElErrorSeBorraAlVolverACargar()
    {
      await controlador.IniciarAsync();
      servidor.Caido = true;
      await vista.SeleccionarMagnitud("Masa");
      servidor.Caido = false;

      await vista.SeleccionarMagnitud("Masa");

      Assert.Null(vista.Error);
      Assert.Equal(["g", "kg"], vista.Unidades);
    }
  }
}
