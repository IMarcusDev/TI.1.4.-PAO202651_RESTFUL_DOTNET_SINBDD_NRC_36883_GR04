namespace WS_CON_UNI_DOTNET_SOAP.Models.Interfaces
{
  using System.Collections.Generic;

  public interface IDataConverter
  {
    Dictionary<string, double> ToMap();

    double Convert(double value, string from, string to)
    {
      Dictionary<string, double> map = ToMap();

      if (!map.ContainsKey(from) || !map.ContainsKey(to))
        return 0;

      double valueFrom = map[from];
      double valueTo = map[to];

      return value * valueFrom / valueTo;
    }

    IEnumerable<string> GetUnits()
    {
      return ToMap().Keys;
    }
  }
}
