using ec.edu.monster.controlador;
using ec.edu.monster.modelo;

namespace ec.edu.monster.pruebas
{
  public class LoginControladorTest
  {
    private readonly ServidorFalso servidor = new();
    private readonly Sesion sesion = new();
    private readonly VistaLoginFalsa vista = new();

    public LoginControladorTest()
    {
      _ = new LoginControlador(vista, new ConversorServicio(servidor, servidor, sesion));
    }

    [Fact]
    public async Task ElLoginCorrectoAbreElMenu()
    {
      vista.Usuario = " user ";
      vista.Clave = "1234";

      await vista.PresionarIngresar();

      Assert.True(vista.MenuAbierto);
      Assert.Null(vista.Error);
      Assert.Equal("user", sesion.Usuario);
    }

    [Theory]
    [InlineData("", "1234")]
    [InlineData("user", "")]
    [InlineData("   ", "   ")]
    public async Task SinUsuarioOClaveNoSeLlamaAlServidor(string usuario, string clave)
    {
      servidor.Caido = true;
      vista.Usuario = usuario;
      vista.Clave = clave;

      await vista.PresionarIngresar();

      Assert.Equal(LoginControlador.CredencialesVacias, vista.Error);
      Assert.False(vista.MenuAbierto);
    }

    [Fact]
    public async Task ElLoginIncorrectoMuestraElMensajeDelServidor()
    {
      vista.Usuario = "user";
      vista.Clave = "mala";

      await vista.PresionarIngresar();

      Assert.Equal("Credenciales Incorrectas", vista.Error);
      Assert.False(vista.MenuAbierto);
      Assert.False(sesion.EstaActiva());
    }

    [Fact]
    public async Task ConElServidorCaidoMuestraErrorDeConexion()
    {
      servidor.Caido = true;
      vista.Usuario = "user";
      vista.Clave = "1234";

      await vista.PresionarIngresar();

      Assert.Equal(ConversorServicio.SinConexion, vista.Error);
      Assert.False(vista.MenuAbierto);
    }
  }
}
