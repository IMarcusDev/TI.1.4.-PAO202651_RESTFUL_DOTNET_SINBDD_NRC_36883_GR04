using ec.edu.monster.modelo;

namespace ec.edu.monster.pruebas
{
  public class ConfiguracionTest : IDisposable
  {
    private readonly string carpeta = Directory.CreateTempSubdirectory("esc-config-").FullName;

    public void Dispose() => Directory.Delete(carpeta, recursive: true);

    private void Escribir(string json) =>
      File.WriteAllText(Path.Combine(carpeta, Configuracion.Archivo), json);

    [Fact]
    public void LeeLaDireccionDelArchivo()
    {
      Escribir("{ \"UrlBase\": \"http://192.168.1.20:5052/\" }");

      Assert.Equal("http://192.168.1.20:5052", Configuracion.LeerUrlServidor(carpeta));
    }

    [Fact]
    public void SinArchivoUsaLaDireccionPorDefecto()
    {
      Assert.Equal(Configuracion.UrlPorDefecto, Configuracion.LeerUrlServidor(carpeta));
    }

    [Theory]
    [InlineData("no es json")]
    [InlineData("{ }")]
    [InlineData("{ \"UrlBase\": 5052 }")]
    [InlineData("{ \"UrlBase\": \"localhost\" }")]
    [InlineData("{ \"UrlBase\": \"ftp://servidor\" }")]
    public void UnArchivoInvalidoUsaLaDireccionPorDefecto(string json)
    {
      Escribir(json);

      Assert.Equal(Configuracion.UrlPorDefecto, Configuracion.LeerUrlServidor(carpeta));
    }
  }
}
