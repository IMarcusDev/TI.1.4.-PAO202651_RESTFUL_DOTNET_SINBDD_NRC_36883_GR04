using System.Net;
using ec.edu.monster.modelo;

namespace ec.edu.monster.pruebas
{
  /// <summary>
  /// Conexión real (ClienteRest y EstadoServidor): contra una dirección donde no hay
  /// nadie escuchando y contra un transporte HTTP en memoria.
  /// </summary>
  public class ConexionServidorTest
  {
    // Puerto 9 (discard): nadie escucha ahí, así que la conexión falla enseguida.
    private const string SinServidor = "http://127.0.0.1:9";

    private readonly ManejadorFalso manejador = new();

    [Fact]
    public async Task SinServidorNoEstaDisponible()
    {
      Assert.False(await new EstadoServidor(SinServidor).DisponibleAsync());
    }

    [Fact]
    public async Task SinServidorElLoginEsErrorDeConexion()
    {
      var servicio = ClienteRest.CrearServicio(SinServidor, new Sesion());

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

      Assert.True(await new EstadoServidor("http://servidor:5013", manejador).DisponibleAsync());
      Assert.Equal("http://servidor:5013/catalog/longitud", manejador.Uri!.ToString());
      Assert.False(manejador.Peticion!.Headers.Contains("Authorization"));
    }

    [Theory]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.InternalServerError)]
    public async Task OtroServidorQueNoEsElRestfulNoEstaDisponible(HttpStatusCode estado)
    {
      manejador.Estado = estado;

      Assert.False(await new EstadoServidor("http://servidor:5013", manejador).DisponibleAsync());
    }

    [Fact]
    public async Task UnaPeticionQueExcedeElTiempoNoEstaDisponible()
    {
      manejador.Falla = new TaskCanceledException("Tiempo agotado");

      Assert.False(await new EstadoServidor("http://servidor:5013", manejador).DisponibleAsync());
    }

    [Fact]
    public async Task ElServicioCreadoUsaLaUrlBaseIndicada()
    {
      manejador.Cuerpo = "{\"estado\":0,\"mensaje\":\"ok\",\"token\":\"t-9\"}";
      var sesion = new Sesion();

      await ClienteRest.CrearServicio("http://servidor:5013", sesion, manejador).IniciarSesionAsync("user", "1234");

      Assert.Equal("http://servidor:5013/login", manejador.Uri!.ToString());
      Assert.Equal("t-9", sesion.Token);
    }
  }
}
