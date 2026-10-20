using CatalogServiceRef;
using CONSOLE_CON_UNI_DOTNET_SOAP.Models.Enums;
using CONSOLE_CON_UNI_DOTNET_SOAP.Models.HTTP;
using System;
using System.Collections.Generic;
using System.Text;

namespace CONSOLE_CON_UNI_DOTNET_SOAP.Controllers
{
  internal class CatalogController
  {
    private TypesEnum type;
    private CatalogHTTP http;

    public CatalogController(TypesEnum type)
    {
      this.type = type;
      this.http = new CatalogHTTP();
    }

    public CatalogResponseDTO? GetCatalog()
    {
      CatalogResponseDTO? dto = this.http.GetCatalog(
        ConfigHTTP.token,
        this.type.ToString()
      ).Result;

      return dto;
    }
  }
}
