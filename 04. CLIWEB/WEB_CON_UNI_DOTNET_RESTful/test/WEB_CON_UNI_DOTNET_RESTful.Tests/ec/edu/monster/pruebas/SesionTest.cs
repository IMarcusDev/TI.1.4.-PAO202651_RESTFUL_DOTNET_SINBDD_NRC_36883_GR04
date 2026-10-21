using ec.edu.monster.modelo;

namespace ec.edu.monster.pruebas
{
  public class SesionTest
  {
    [Fact]
    public void UnaSesionNuevaNoEstaActiva()
    {
      Assert.False(new Sesion().EstaActiva());
    }

    [Fact]
    public void IniciarYCerrar()
    {
      var sesion = new Sesion();

      sesion.Iniciar("user", "abc");
      Assert.True(sesion.EstaActiva());
      Assert.Equal("user", sesion.Usuario);
      Assert.Equal("abc", sesion.Token);

      sesion.Cerrar();
      Assert.False(sesion.EstaActiva());
      Assert.Null(sesion.Usuario);
      Assert.Null(sesion.Token);
    }
  }
}
