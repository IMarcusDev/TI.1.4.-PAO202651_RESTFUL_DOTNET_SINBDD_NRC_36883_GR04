using ec.edu.monster.controlador;

namespace ec.edu.monster.pruebas
{
  public class EstadoControladorTest
  {
    private readonly ServidorFalso servidor = new();
    private readonly VistaEstadoFalsa vista = new();
    private readonly EstadoFalso estado;

    public EstadoControladorTest()
    {
      estado = new EstadoFalso(servidor);
      _ = new EstadoControlador(vista, estado);
    }

    [Fact]
    public async Task ConElServidorArribaMuestraVerde()
    {
      await vista.Revisar();

      Assert.True(vista.Disponible);
    }

    [Fact]
    public async Task CuandoElServidorSeCaeCambiaARojoYVuelveAVerde()
    {
      await vista.Revisar();
      servidor.Caido = true;

      await vista.Revisar();
      Assert.False(vista.Disponible);

      servidor.Caido = false;
      await vista.Revisar();
      Assert.True(vista.Disponible);
      Assert.Equal(3, estado.Consultas);
    }
  }
}
