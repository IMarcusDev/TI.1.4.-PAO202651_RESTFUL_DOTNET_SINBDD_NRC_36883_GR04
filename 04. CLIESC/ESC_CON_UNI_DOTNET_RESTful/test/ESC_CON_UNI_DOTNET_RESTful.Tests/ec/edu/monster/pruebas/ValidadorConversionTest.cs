using ec.edu.monster.controlador;

namespace ec.edu.monster.pruebas
{
  public class ValidadorConversionTest
  {
    [Theory]
    [InlineData("2.5", 2.5)]
    [InlineData("2,5", 2.5)]
    [InlineData(" 10 ", 10)]
    [InlineData("-3", -3)]
    [InlineData("+.5", 0.5)]
    public void LeeDecimalesSimples(string texto, double esperado)
    {
      Assert.Equal(esperado, ValidadorConversion.LeerValor(texto));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("1e5")]
    [InlineData("1.2.3")]
    [InlineData("1,000.5")]
    [InlineData("5m")]
    [InlineData("NaN")]
    [InlineData("Infinity")]
    public void RechazaTextoQueNoEsUnNumeroSimple(string? texto)
    {
      Assert.Null(ValidadorConversion.LeerValor(texto));
    }

    [Fact]
    public void RechazaNumerosDemasiadoGrandes()
    {
      Assert.Null(ValidadorConversion.LeerValor(new string('9', 400)));
    }

    [Fact]
    public void ValidaEnOrdenValorMagnitudYUnidades()
    {
      Assert.Equal(ValidadorConversion.ValorInvalido, ValidadorConversion.Validar("x", null, null, null));
      Assert.Equal(ValidadorConversion.SinMagnitud, ValidadorConversion.Validar("1", "Velocidad", "m", "cm"));
      Assert.Equal(ValidadorConversion.SinUnidades, ValidadorConversion.Validar("1", "Longitud", "", "cm"));
      Assert.Equal(ValidadorConversion.SinUnidades, ValidadorConversion.Validar("1", "Longitud", "m", null));
      Assert.Null(ValidadorConversion.Validar("1", "Longitud", "m", "cm"));
    }
  }
}
