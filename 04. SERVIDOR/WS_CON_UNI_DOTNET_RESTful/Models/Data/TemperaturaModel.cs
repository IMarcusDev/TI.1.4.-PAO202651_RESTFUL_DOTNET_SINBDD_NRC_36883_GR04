using WS_CON_UNI_DOTNET_SOAP.Models.Interfaces;

namespace WS_CON_UNI_DOTNET_SOAP.Models.Data
{
  public class TemperaturaModel : IDataConverter
  {
    public Dictionary<string, double> ToMap()
    {
      Dictionary<string, double> data = new()
        {
            { "°C", 1.0 },
            { "°F", 1.0 },
            { "K", 1.0 },
            { "°N", 1.0 },
            { "°Ré", 1.0 }
        };

      return data;
    }

    public double Convert(double value, string from, string to)
    {
      Dictionary<string, double> map = ToMap();

      if (!map.ContainsKey(from) || !map.ContainsKey(to))
        return 0;

      double celsius;

      switch (from)
      {
        case "°C":
          celsius = value;
          break;
        case "°F":
          celsius = (value - 32) * 5.0 / 9.0;
          break;
        case "K":
          celsius = value - 273.15;
          break;
        case "°N":
          celsius = value * 100.0 / 33.0;
          break;
        case "°Ré":
          celsius = value * 5.0 / 4.0;
          break;
        default:
          return 0;
      }

      switch (to)
      {
        case "°C":
          return celsius;
        case "°F":
          return celsius * 9.0 / 5.0 + 32;
        case "K":
          return celsius + 273.15;
        case "°N":
          return celsius * 33.0 / 100.0;
        case "°Ré":
          return celsius * 4.0 / 5.0;
        default:
          return 0;
      }
    }
  }
}
