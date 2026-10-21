using ec.edu.monster.controlador;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.WebEncoders;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

// Todas las peticiones POST exigen el token antiforgery del formulario.
builder.Services.AddControllersWithViews(opciones =>
  opciones.Filters.Add(new AutoValidateAntiforgeryTokenAttribute()));

// Las vistas viven en ec/edu/monster/vista, igual que en el cliente web de Java.
builder.Services.Configure<RazorViewEngineOptions>(opciones =>
{
  opciones.ViewLocationFormats.Clear();
  opciones.ViewLocationFormats.Add("/ec/edu/monster/vista/{0}.cshtml");
  opciones.ViewLocationFormats.Add("/ec/edu/monster/vista/fragmentos/{0}.cshtml");
});

// Las tildes y símbolos (Área, °C) se escriben tal cual en el HTML, sin códigos &#x...;.
builder.Services.Configure<WebEncoderOptions>(opciones =>
  opciones.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(opciones =>
{
  opciones.IdleTimeout = TimeSpan.FromMinutes(30);
  opciones.Cookie.HttpOnly = true;
  opciones.Cookie.IsEssential = true;
});

string urlServidor = builder.Configuration["ServidorRest:UrlBase"] ?? "http://localhost:5013";
builder.Services.AddSingleton<IFabricaServicios>(new FabricaServicios(urlServidor));

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.MapControllers();

app.Run();

// Permite que las pruebas de integración levanten la aplicación.
public partial class Program { }
