using CONSOLE_CON_UNI_DOTNET_SOAP.Models.Enums;
using CONSOLE_CON_UNI_DOTNET_SOAP.Models.HTTP;
using ConverterServiceRef;
using System;
using System.Collections.Generic;
using System.Text;

namespace CONSOLE_CON_UNI_DOTNET_SOAP.Controllers
{
  internal class ConversorController
  {
    private TypesEnum type;
    private ConversorHTTP http;

    public ConversorController(TypesEnum type)
    {
      this.type = type;
      this.http = new ConversorHTTP();
    }

    public ConverterResponseDTO? MakeConversion(double value, string from, string to)
    {
      ConverterResponseDTO? dto = this.http.Convert(
        ConfigHTTP.token,
        this.type.ToString(),
        value,
        from,
        to
      ).Result;

      return dto;
    }
  }
}
