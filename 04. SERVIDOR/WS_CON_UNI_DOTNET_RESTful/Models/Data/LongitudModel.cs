using WS_CON_UNI_DOTNET_SOAP.Models.Interfaces;

namespace WS_CON_UNI_DOTNET_SOAP.Models.Data
{
  public class LongitudModel : IDataConverter
  {
    // private readonly double kilometer = 1000;
    // private readonly double hectometer = 100;
    // private readonly double decameter = 10;
    private readonly double meter = 1;
    // private readonly double decimeter = 0.1;
    private readonly double centimeter = 0.01;
    // private readonly double millimeter = 0.001;
    private readonly double feet = 0.3048;
    private readonly double yard = 0.9144;
    private readonly double mile = 1609.344;

    public Dictionary<string, double> ToMap()
    {
      Dictionary<string, double> data = new();

      // data.Add("km", kilometer);
      // data.Add("hm", hectometer);
      // data.Add("Dm", decameter);
      data.Add("m", meter);
      // data.Add("dm", decimeter);
      data.Add("cm", centimeter);
      // data.Add("mm", millimeter);
      data.Add("ft", feet);
      data.Add("yd", yard);
      data.Add("mi", mile);

      return data;
    }
  }
}
