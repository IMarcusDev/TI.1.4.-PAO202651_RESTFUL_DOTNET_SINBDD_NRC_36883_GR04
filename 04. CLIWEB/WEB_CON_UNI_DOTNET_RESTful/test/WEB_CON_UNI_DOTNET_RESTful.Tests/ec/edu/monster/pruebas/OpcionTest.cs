using ec.edu.monster.controlador;

namespace ec.edu.monster.pruebas
{
  public class OpcionTest
  {
    [Fact]
    public void LasMagnitudesLlevanSuTextoVisible()
    {
      var magnitudes = Opcion.Magnitudes();

      Assert.Equal(6, magnitudes.Count);
      Assert.Equal(new Opcion("Longitud", "Longitud"), magnitudes[0]);
      Assert.Equal(new Opcion("Area", "Área"), magnitudes[5]);
    }

    [Fact]
    public void LasUnidadesLlevanSuNombreEnEspanol()
    {
      Assert.Equal(
        [new Opcion("m", "Metro"), new Opcion("°F", "Fahrenheit"), new Opcion("zz", "zz")],
        Opcion.Unidades(["m", "°F", "zz"]));
    }
  }
}
