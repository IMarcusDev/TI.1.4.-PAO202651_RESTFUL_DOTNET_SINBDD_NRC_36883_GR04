using ec.edu.monster.modelo;
using ec.edu.monster.modelo.dto;
using ec.edu.monster.modelo.http;

namespace ec.edu.monster.pruebas
{
  public class ConversorServicioTest
  {
    private readonly ServidorFalso servidor = new();
    private readonly Sesion sesion = new();

    private ConversorServicio Servicio() => new(servidor, servidor, sesion);

    [Fact]
    public async Task ElLoginCorrectoGuardaElToken()
    {
      await Servicio().IniciarSesionAsync("user", "1234");

      Assert.True(sesion.EstaActiva());
      Assert.Equal("user", sesion.Usuario);
      Assert.Equal(ServidorFalso.Token, sesion.Token);
    }

    [Fact]
    public async Task ElLoginIncorrectoMuestraElMensajeDelServidorYCierraLaSesion()
    {
      sesion.Iniciar("anterior", "viejo");

      var error = await Assert.ThrowsAsync<ServicioException>(() => Servicio().IniciarSesionAsync("user", "mal"));

      Assert.Equal("Credenciales Incorrectas", error.Message);
      Assert.False(sesion.EstaActiva());
    }

    [Fact]
    public async Task ConvierteConElTokenDeLaSesion()
    {
      await Servicio().IniciarSesionAsync("user", "1234");

      double resultado = await Servicio().ConvertirAsync("Longitud", 2.5, "m", "cm");

      Assert.Equal(250, resultado);
      Assert.Equal((ServidorFalso.Token, "Longitud", 2.5, "m", "cm"), servidor.UltimaConversion);
    }

    [Fact]
    public async Task ElCatalogoDevuelveLasUnidades()
    {
      await Servicio().IniciarSesionAsync("user", "1234");

      Assert.Equal(["m", "cm", "ft"], await Servicio().CatalogoAsync("Longitud"));
    }

    [Fact]
    public async Task SinSesionNoSeLlamaAlServidor()
    {
      var error = await Assert.ThrowsAsync<ServicioException>(() => Servicio().CatalogoAsync("Longitud"));

      Assert.Equal(ConversorServicio.SinSesion, error.Message);
      Assert.Null(servidor.UltimaConversion);
    }

    [Fact]
    public async Task UnTokenRechazadoMuestraElMensajeDelServidor()
    {
      sesion.Iniciar("user", "token-vencido");

      var error = await Assert.ThrowsAsync<ServicioException>(() => Servicio().ConvertirAsync("Masa", 1, "g", "kg"));

      Assert.Equal("Token Inválido", error.Message);
    }

    [Fact]
    public async Task UnServidorCaidoEsErrorDeConexion()
    {
      servidor.Caido = true;

      var error = await Assert.ThrowsAsync<ServicioException>(() => Servicio().IniciarSesionAsync("user", "1234"));

      Assert.Equal(ConversorServicio.SinConexion, error.Message);
    }

    [Fact]
    public async Task UnaPeticionQueExcedeElTiempoEsErrorDeConexion()
    {
      sesion.Iniciar("user", ServidorFalso.Token);
      var servicio = new ConversorServicio(servidor, new ConversorLento(), sesion);

      var error = await Assert.ThrowsAsync<ServicioException>(() => servicio.ConvertirAsync("Masa", 1, "g", "kg"));

      Assert.Equal(ConversorServicio.SinConexion, error.Message);
    }

    [Fact]
    public async Task UnaRespuestaQueNoEsJsonSeReportaComoSinRespuesta()
    {
      sesion.Iniciar("user", ServidorFalso.Token);
      servidor.RespuestaNoJson = true;

      var error = await Assert.ThrowsAsync<ServicioException>(() => Servicio().CatalogoAsync("Masa"));

      Assert.Equal(ConversorServicio.SinRespuesta, error.Message);
    }

    [Fact]
    public async Task UnaRespuestaVaciaSeReporta()
    {
      servidor.SinRespuesta = true;

      var error = await Assert.ThrowsAsync<ServicioException>(() => Servicio().IniciarSesionAsync("user", "1234"));

      Assert.Equal(ConversorServicio.SinRespuesta, error.Message);
    }

    [Fact]
    public async Task UnLoginSinTokenSeReporta()
    {
      var sinToken = new LoginSinToken();
      var servicio = new ConversorServicio(sinToken, servidor, sesion);

      var error = await Assert.ThrowsAsync<ServicioException>(() => servicio.IniciarSesionAsync("user", "1234"));

      Assert.Equal(ConversorServicio.SinToken, error.Message);
      Assert.False(sesion.EstaActiva());
    }

    [Fact]
    public async Task UnMensajeVacioDelServidorUsaElMensajeGenerico()
    {
      sesion.Iniciar("user", ServidorFalso.Token);
      var servicio = new ConversorServicio(servidor, new ConversorSinMensaje(), sesion);

      var error = await Assert.ThrowsAsync<ServicioException>(() => servicio.CatalogoAsync("Masa"));

      Assert.Equal(ConversorServicio.ErrorGenerico, error.Message);
    }

    [Fact]
    public void CerrarSesionBorraElToken()
    {
      sesion.Iniciar("user", "abc");

      Servicio().CerrarSesion();

      Assert.False(sesion.EstaActiva());
    }

    // Un servidor que no responde a tiempo: HttpClient lo informa como TaskCanceledException.
    private class ConversorLento : IConversorApi
    {
      public Task<ConversorResponseDTO> ConvertAsync(string token, string type, double value, string from, string to) =>
        throw new TaskCanceledException("Tiempo agotado");

      public Task<CatalogResponseDTO> CatalogAsync(string token, string type) =>
        throw new TaskCanceledException("Tiempo agotado");
    }

    private class LoginSinToken : ILoginApi
    {
      public Task<LoginResponseDTO> LoginAsync(string usuario, string clave) =>
        Task.FromResult(new LoginResponseDTO(modelo.enums.EstadosEnum.OK, "ok", " "));
    }

    private class ConversorSinMensaje : IConversorApi
    {
      public Task<ConversorResponseDTO> ConvertAsync(string token, string type, double value, string from, string to) =>
        Task.FromResult(new ConversorResponseDTO(modelo.enums.EstadosEnum.ERROR, "  ", 0));

      public Task<CatalogResponseDTO> CatalogAsync(string token, string type) =>
        Task.FromResult(new CatalogResponseDTO(modelo.enums.EstadosEnum.ERROR, null, null));
    }
  }
}
