using System.Net;
using ec.edu.monster.controlador;
using ec.edu.monster.modelo;

namespace ec.edu.monster.pruebas
{
  public class FabricaServiciosTest
  {
    // Puerto 9 (discard): nadie escucha ahí, así que la conexión falla enseguida.
    private const string SinServidor = "http://127.0.0.1:9";

    private readonly ManejadorFalso manejador = new();

    [Fact]
    public async Task SinServidorNoEstaDisponible()
    {
      Assert.False(await new FabricaServicios(SinServidor).DisponibleAsync());
    }

    [Fact]
    public async Task SinServidorElLoginEsErrorDeConexion()
    {
      var servicio = new FabricaServicios(SinServidor).CrearServicio(new Sesion());

      var error = await Assert.ThrowsAsync<ServicioException>(() => servicio.IniciarSesionAsync("user", "1234"));

      Assert.Equal(ConversorServicio.SinConexion, error.Message);
    }

    // Sin token el servidor vivo responde 401 con su JSON: eso basta para saber que está arriba.
    [Theory]
    [InlineData(HttpStatusCode.OK)]
    [InlineData(HttpStatusCode.Unauthorized)]
    public async Task UnServidorQueRespondeEstaDisponible(HttpStatusCode estado)
    {
      manejador.Estado = estado;

      Assert.True(await new FabricaServicios("http://servidor:5013", manejador).DisponibleAsync());
      Assert.Equal("http://servidor:5013/catalog/longitud", manejador.Uri!.ToString());
      Assert.False(manejador.Peticion!.Headers.Contains("Authorization"));
    }

    [Theory]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task OtroServidorQueNoEsElRestfulNoEstaDisponible(HttpStatusCode estado)
    {
      manejador.Estado = estado;

      Assert.False(await new FabricaServicios("http://servidor:5013", manejador).DisponibleAsync());
    }

    [Fact]
    public async Task UnaPeticionQueExcedeElTiempoNoEstaDisponible()
    {
      manejador.Falla = new TaskCanceledException("Tiempo agotado");

      Assert.False(await new FabricaServicios("http://servidor:5013", manejador).DisponibleAsync());
    }

    [Fact]
    public async Task ElServicioCreadoUsaLaUrlBaseIndicada()
    {
      manejador.Cuerpo = "{\"estado\":0,\"mensaje\":\"ok\",\"token\":\"t-9\"}";
      var sesion = new Sesion();

      await new FabricaServicios("http://servidor:5013", manejador).CrearServicio(sesion)
        .IniciarSesionAsync("user", "1234");

      Assert.Equal("http://servidor:5013/login", manejador.Uri!.ToString());
      Assert.Equal("t-9", sesion.Token);
    }
  }
}
