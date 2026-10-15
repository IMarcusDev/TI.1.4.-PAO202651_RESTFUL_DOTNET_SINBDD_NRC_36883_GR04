using WS_CON_UNI_DOTNET_SOAP.Models.Interfaces;

namespace WS_CON_UNI_DOTNET_SOAP.Models.Data
{
  public class AreaModel : IDataConverter
  {
    private readonly double squareCentimeter = 0.0001;
    private readonly double squareMeter = 1;
    private readonly double squareKilometer = 1000000;
    private readonly double hectare = 10000;
    private readonly double squareFeet = 0.092903;
    private readonly double acre = 4046.86;

    public Dictionary<string, double> ToMap()
    {
      Dictionary<string, double> data = new Dictionary<string, double>();

      data.Add("cm2", squareCentimeter);
      data.Add("m2", squareMeter);
      data.Add("km2", squareKilometer);
      data.Add("ha", hectare);
      data.Add("ft2", squareFeet);
      data.Add("ac", acre);

      return data;
    }
  }
}
