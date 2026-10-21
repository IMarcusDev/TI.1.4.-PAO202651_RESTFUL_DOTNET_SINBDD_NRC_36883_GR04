using System.Net;
using System.Text.RegularExpressions;
using ec.edu.monster.controlador;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace ec.edu.monster.pruebas
{
  /// <summary>
  /// Levanta la aplicación web completa en memoria, conectada al servidor falso.
  /// </summary>
  public class WebFixture : WebApplicationFactory<Program>
  {
    public ServidorFalso Servidor { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
      builder.ConfigureTestServices(servicios =>
      {
        servicios.AddSingleton<IFabricaServicios>(new FabricaFalsa(Servidor));
      });
    }

    /// <summary>Cliente con cookies propias que no sigue redirecciones.</summary>
    public HttpClient NuevoCliente()
    {
      return CreateClient(new WebApplicationFactoryClientOptions
      {
        AllowAutoRedirect = false,
        HandleCookies = true,
      });
    }

    /// <summary>Hace GET a la página y envía su formulario POST con el token antiforgery.</summary>
    public static async Task<HttpResponseMessage> EnviarAsync(
      HttpClient cliente, string pagina, string accion, Dictionary<string, string> campos)
    {
      string html = await cliente.GetStringAsync(pagina);
      campos["__RequestVerificationToken"] = TokenAntiforgery(html);
      return await cliente.PostAsync(accion, new FormUrlEncodedContent(campos));
    }

    public static async Task<HttpClient> ClienteConSesionAsync(WebFixture fixture)
    {
      var cliente = fixture.NuevoCliente();
      var respuesta = await EnviarAsync(cliente, "/login", "/login",
        new() { ["usuario"] = "user", ["clave"] = "1234" });
      Assert.Equal(HttpStatusCode.Redirect, respuesta.StatusCode);
      return cliente;
    }

    public static string TokenAntiforgery(string html)
    {
      var coincidencia = Regex.Match(html, "name=\"__RequestVerificationToken\" type=\"hidden\" value=\"([^\"]+)\"");
      Assert.True(coincidencia.Success, "La página no tiene token antiforgery");
      return coincidencia.Groups[1].Value;
    }
  }
}
