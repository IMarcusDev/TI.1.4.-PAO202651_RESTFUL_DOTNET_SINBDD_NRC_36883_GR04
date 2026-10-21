using System.Net;
using ec.edu.monster.controlador;

namespace ec.edu.monster.pruebas
{
  /// <summary>
  /// Recorre la aplicación web completa (rutas, filtros, sesión y vistas)
  /// contra el servidor RESTful en memoria.
  /// </summary>
  public class AplicacionWebTest : IDisposable
  {
    private readonly WebFixture app = new();

    public void Dispose() => app.Dispose();

    [Fact]
    public async Task LaRaizLlevaAlLogin()
    {
      var respuesta = await app.NuevoCliente().GetAsync("/");

      Assert.Equal(HttpStatusCode.Redirect, respuesta.StatusCode);
      Assert.Equal("/login", respuesta.Headers.Location?.OriginalString);
    }

    [Theory]
    [InlineData("/menu")]
    [InlineData("/conversion")]
    [InlineData("/catalogo")]
    [InlineData("/logout")]
    public async Task LasPaginasProtegidasPidenLogin(string pagina)
    {
      var respuesta = await app.NuevoCliente().GetAsync(pagina);

      Assert.Equal(HttpStatusCode.Redirect, respuesta.StatusCode);
      Assert.Equal("/login", respuesta.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task ElLoginMuestraElEstadoDelServicio()
    {
      var cliente = app.NuevoCliente();
      Assert.Contains("Servicio disponible", await cliente.GetStringAsync("/login"));

      app.Servidor.Caido = true;
      Assert.Contains("Servicio no disponible", await cliente.GetStringAsync("/login"));
    }

    [Fact]
    public async Task ElLoginExigeUsuarioYClave()
    {
      var respuesta = await WebFixture.EnviarAsync(app.NuevoCliente(), "/login", "/login",
        new() { ["usuario"] = "user", ["clave"] = "" });
      string html = await respuesta.Content.ReadAsStringAsync();

      Assert.Equal(HttpStatusCode.OK, respuesta.StatusCode);
      Assert.Contains(LoginController.CredencialesVacias, html);
      Assert.Contains("value=\"user\"", html);
    }

    [Fact]
    public async Task ElLoginIncorrectoMuestraElDialogoDeError()
    {
      var respuesta = await WebFixture.EnviarAsync(app.NuevoCliente(), "/login", "/login",
        new() { ["usuario"] = "user", ["clave"] = "mala" });
      string html = await respuesta.Content.ReadAsStringAsync();

      Assert.Contains("Error de autenticación", html);
      Assert.Contains("Credenciales Incorrectas", html);
      Assert.DoesNotContain("mala", html);
    }

    [Fact]
    public async Task ElPostSinTokenAntiforgerySeRechaza()
    {
      var respuesta = await app.NuevoCliente().PostAsync("/login",
        new FormUrlEncodedContent(new Dictionary<string, string> { ["usuario"] = "user", ["clave"] = "1234" }));

      Assert.Equal(HttpStatusCode.BadRequest, respuesta.StatusCode);
    }

    [Fact]
    public async Task ElLoginCorrectoLlevaAlMenu()
    {
      var cliente = await WebFixture.ClienteConSesionAsync(app);

      string menu = await cliente.GetStringAsync("/menu");
      Assert.Contains("Menú principal", menu);
      Assert.Contains("Sesión activa", menu);

      // Con sesión activa, /login vuelve al menú.
      var login = await cliente.GetAsync("/login");
      Assert.Equal("/menu", login.Headers.Location?.OriginalString);
    }

    [Theory]
    [InlineData("/menu")]
    [InlineData("/conversion")]
    [InlineData("/catalogo")]
    public async Task ConSesionYServidorCaidoLaCabeceraMuestraElPuntoRojo(string pagina)
    {
      var cliente = await WebFixture.ClienteConSesionAsync(app);
      app.Servidor.Caido = true;

      string html = await cliente.GetStringAsync(pagina);

      Assert.Contains("estado-punto inactivo", html);
      Assert.Contains("<span class=\"estado-texto\">Servidor no disponible</span>", html);
      Assert.DoesNotContain("<span class=\"estado-texto\">Sesión activa</span>", html);
    }

    [Fact]
    public async Task ConSesionYServidorArribaLaCabeceraMuestraElPuntoVerde()
    {
      var cliente = await WebFixture.ClienteConSesionAsync(app);

      string html = await cliente.GetStringAsync("/menu");

      Assert.DoesNotContain("estado-punto inactivo", html);
      Assert.Contains("<span class=\"estado-texto\">Sesión activa</span>", html);
    }

    [Fact]
    public async Task ElEstadoDelServidorSeConsultaSinRecargar()
    {
      var cliente = app.NuevoCliente();

      Assert.Equal("{\"disponible\":true}", await cliente.GetStringAsync("/estado"));

      app.Servidor.Caido = true;
      var respuesta = await cliente.GetAsync("/estado");

      Assert.Equal("{\"disponible\":false}", await respuesta.Content.ReadAsStringAsync());
      Assert.True(respuesta.Headers.CacheControl?.NoStore);
    }

    [Fact]
    public async Task LasPaginasProtegidasNoSeGuardanEnCache()
    {
      var cliente = await WebFixture.ClienteConSesionAsync(app);

      var respuesta = await cliente.GetAsync("/menu");

      Assert.True(respuesta.Headers.CacheControl?.NoStore);
    }

    [Fact]
    public async Task ElCatalogoListaLasUnidadesDeLaMagnitud()
    {
      var cliente = await WebFixture.ClienteConSesionAsync(app);

      string html = await cliente.GetStringAsync("/catalogo?magnitud=Temperatura");

      Assert.Contains("<td>Celsius</td>", html);
      Assert.Contains("<td>°F</td>", html);
      Assert.DoesNotContain("<td>Metro</td>", html);
    }

    [Fact]
    public async Task UnaMagnitudDesconocidaEnElCatalogoMuestraLaPrimera()
    {
      var cliente = await WebFixture.ClienteConSesionAsync(app);

      string html = await cliente.GetStringAsync("/catalogo?magnitud=Velocidad");

      Assert.Contains("<td>Metro</td>", html);
    }

    [Fact]
    public async Task LaConversionMuestraElResultadoConFormato()
    {
      var cliente = await WebFixture.ClienteConSesionAsync(app);

      var respuesta = await WebFixture.EnviarAsync(cliente, "/conversion", "/conversion", new()
      {
        ["magnitud"] = "Longitud", ["desde"] = "m", ["hacia"] = "cm", ["valor"] = "2,5", ["accion"] = "convertir",
      });
      string html = await respuesta.Content.ReadAsStringAsync();

      Assert.Contains("2,5 m = 250,0000 cm", html);
      Assert.Equal((ServidorFalso.Token, "Longitud", 2.5, "m", "cm"), app.Servidor.UltimaConversion);
    }

    [Fact]
    public async Task UnValorInvalidoNoLlamaAlServidor()
    {
      var cliente = await WebFixture.ClienteConSesionAsync(app);

      var respuesta = await WebFixture.EnviarAsync(cliente, "/conversion", "/conversion", new()
      {
        ["magnitud"] = "Longitud", ["desde"] = "m", ["hacia"] = "cm", ["valor"] = "abc", ["accion"] = "convertir",
      });

      Assert.Contains(ValidadorConversion.ValorInvalido, await respuesta.Content.ReadAsStringAsync());
      Assert.Null(app.Servidor.UltimaConversion);
    }

    [Fact]
    public async Task UnidadesDeOtraMagnitudNoSeConvierten()
    {
      var cliente = await WebFixture.ClienteConSesionAsync(app);

      var respuesta = await WebFixture.EnviarAsync(cliente, "/conversion", "/conversion", new()
      {
        ["magnitud"] = "Masa", ["desde"] = "m", ["hacia"] = "cm", ["valor"] = "1", ["accion"] = "convertir",
      });

      Assert.Contains(ValidadorConversion.SinUnidades, await respuesta.Content.ReadAsStringAsync());
      Assert.Null(app.Servidor.UltimaConversion);
    }

    [Fact]
    public async Task CambiarLaMagnitudRecargaUnidadesSinConvertir()
    {
      var cliente = await WebFixture.ClienteConSesionAsync(app);

      var respuesta = await WebFixture.EnviarAsync(cliente, "/conversion", "/conversion", new()
      {
        ["magnitud"] = "Masa", ["desde"] = "m", ["hacia"] = "cm", ["valor"] = "7", ["accion"] = "cambiar",
      });
      string html = await respuesta.Content.ReadAsStringAsync();

      Assert.Contains("Kilogramo (kg)", html);
      Assert.Contains("value=\"7\"", html);
      Assert.DoesNotContain("mensaje-error", html);
      Assert.Null(app.Servidor.UltimaConversion);
    }

    [Fact]
    public async Task SiElServidorSeCaeLaConversionMuestraElError()
    {
      var cliente = await WebFixture.ClienteConSesionAsync(app);
      app.Servidor.Caido = true;

      string html = await cliente.GetStringAsync("/conversion");

      Assert.Contains("No se pudo conectar con el servidor", html);
      Assert.Contains("Sin unidades", html);
    }

    [Fact]
    public async Task CerrarSesionVuelveAlLoginYProtegeLasPaginas()
    {
      var cliente = await WebFixture.ClienteConSesionAsync(app);

      var salir = await WebFixture.EnviarAsync(cliente, "/menu", "/logout", new());
      Assert.Equal("/login", salir.Headers.Location?.OriginalString);

      var menu = await cliente.GetAsync("/menu");
      Assert.Equal("/login", menu.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task LogoutPorGetNoCierraLaSesion()
    {
      var cliente = await WebFixture.ClienteConSesionAsync(app);

      var respuesta = await cliente.GetAsync("/logout");

      Assert.Equal("/menu", respuesta.Headers.Location?.OriginalString);
      Assert.Equal(HttpStatusCode.OK, (await cliente.GetAsync("/menu")).StatusCode);
    }
  }
}
