using ec.edu.monster.modelo;

namespace ec.edu.monster.pruebas
{
  public class NombresUnidadesTest
  {
    [Theory]
    [InlineData("m", "Metro")]
    [InlineData("°C", "Celsius")]
    [InlineData("°Ré", "Réaumur")]
    [InlineData("ft3", "Pie cúbico")]
    [InlineData("ac", "Acre")]
    public void TraduceLosSimbolosConocidos(string simbolo, string nombre)
    {
      Assert.Equal(nombre, NombresUnidades.Nombre(simbolo));
    }

    [Fact]
    public void UnSimboloDesconocidoSeDevuelveIgual()
    {
      Assert.Equal("xyz", NombresUnidades.Nombre("xyz"));
    }
  }
}
