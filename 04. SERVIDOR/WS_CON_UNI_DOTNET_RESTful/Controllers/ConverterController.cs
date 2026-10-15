using WS_CON_UNI_DOTNET_SOAP.Models.Data;
using WS_CON_UNI_DOTNET_SOAP.Models.Interfaces;

namespace WS_CON_UNI_DOTNET_SOAP.Controllers
{
  public class ConverterController
  {
    public static IDataConverter GetConverter(string type)
    {
      switch (type)
      {
        case "Area":
          return new AreaModel();
        case "Longitud":
          return new LongitudModel();
        case "Masa":
          return new MasaPesoModel();
        case "Temperatura":
          return new TemperaturaModel();
        case "Tiempo":
          return new TiempoModel();
        case "Volumen":
          return new VolumenModel();
        default:
          throw new Exception("Type Converter not found!");
      }
    }

    public static double Convert(string type, double value, string from, string to)
    {
      IDataConverter converter = ConverterController.GetConverter(type);

      return converter.Convert(value, from, to);
    }
  }
}
