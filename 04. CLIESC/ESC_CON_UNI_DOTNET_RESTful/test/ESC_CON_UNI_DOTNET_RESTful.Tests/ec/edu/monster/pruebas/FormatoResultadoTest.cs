using ec.edu.monster.modelo;

namespace ec.edu.monster.pruebas
{
  public class FormatoResultadoTest
  {
    [Fact]
    public void UsaComaDecimalYCuatroDecimalesEnElResultado()
    {
      Assert.Equal("2,5 m = 8,2021 ft", FormatoResultado.Formatear(2.5, "m", 8.20209973753, "ft"));
    }

    [Fact]
    public void ElValorDeEntradaNoMuestraCerosSobrantes()
    {
      Assert.Equal("100 °C = 212,0000 °F", FormatoResultado.Formatear(100, "°C", 212, "°F"));
    }

    [Fact]
    public void LosNegativosLlevanSigno()
    {
      Assert.Equal("-40 °C = -40,0000 °F", FormatoResultado.Formatear(-40, "°C", -40, "°F"));
    }
  }
}
