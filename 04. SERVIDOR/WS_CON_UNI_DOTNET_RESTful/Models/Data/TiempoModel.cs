using WS_CON_UNI_DOTNET_SOAP.Models.Interfaces;

namespace WS_CON_UNI_DOTNET_SOAP.Models.Data
{
  public class TiempoModel : IDataConverter
  {
    private readonly double millisecond = 0.001;
    private readonly double second = 1;
    private readonly double minute = 60;
    private readonly double hour = 60 * 60;
    private readonly double day = 60 * 60 * 24;
    private readonly double week = 60 * 60 * 24 * 7;

    public Dictionary<string, double> ToMap()
    {
      Dictionary<string, double> data = new()
        {
            { "ms", millisecond },
            { "s", second },
            { "min", minute },
            { "h", hour },
            { "d", day },
            { "w", week }
        };

      return data;
    }
  }
}
