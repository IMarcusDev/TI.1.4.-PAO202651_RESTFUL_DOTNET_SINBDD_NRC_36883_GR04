using WS_CON_UNI_DOTNET_SOAP.Models.Interfaces;

namespace WS_CON_UNI_DOTNET_SOAP.Models.Data
{
  public class VolumenModel : IDataConverter
  {
    private readonly double milliliter = 0.001;
    private readonly double liter = 1;
    private readonly double cubicMeter = 1000;
    private readonly double cubicFeet = 28.3168;
    private readonly double galon = 3.78541;

    public Dictionary<string, double> ToMap()
    {
      Dictionary<string, double> data = new()
        {
            { "mL", milliliter },
            { "L", liter },
            { "m3", cubicMeter },
            { "ft3", cubicFeet },
            { "gal", galon }
        };

      return data;
    }
  }
}
