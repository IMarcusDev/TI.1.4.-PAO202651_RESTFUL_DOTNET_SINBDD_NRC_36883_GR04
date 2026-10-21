using ec.edu.monster.modelo;

namespace ec.edu.monster.pruebas
{
  public class MagnitudesTest
  {
    [Fact]
    public void TodasEstanEnElOrdenDeLasPantallas()
    {
      Assert.Equal(["Longitud", "Masa", "Volumen", "Tiempo", "Temperatura", "Area"], Magnitudes.Todas);
    }

    [Fact]
    public void AreaSeMuestraConTilde()
    {
      Assert.Equal("Área", Magnitudes.NombreVisible("Area"));
      Assert.Equal("Masa", Magnitudes.NombreVisible("Masa"));
    }

    [Theory]
    [InlineData("Longitud", true)]
    [InlineData("longitud", false)]
    [InlineData("Velocidad", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void ExisteDistingueMayusculas(string? magnitud, bool esperado)
    {
      Assert.Equal(esperado, Magnitudes.Existe(magnitud));
    }
  }
}
