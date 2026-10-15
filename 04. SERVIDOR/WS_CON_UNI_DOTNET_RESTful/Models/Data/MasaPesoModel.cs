using WS_CON_UNI_DOTNET_SOAP.Models.Interfaces;

namespace WS_CON_UNI_DOTNET_SOAP.Models.Data
{
  public class MasaPesoModel : IDataConverter
  {
    private readonly double gram = 0.001;
    private readonly double kilogram = 1;
    private readonly double ounce = 0.0283495;
    private readonly double pound = 0.453592;
    private readonly double quintal = 100;

    public Dictionary<string, double> ToMap()
    {
      Dictionary<string, double> data = new();

      data.Add("g", gram);
      data.Add("kg", kilogram);
      data.Add("oz", ounce);
      data.Add("lb", pound);
      data.Add("qq", quintal);

      return data;
    }
  }
}
