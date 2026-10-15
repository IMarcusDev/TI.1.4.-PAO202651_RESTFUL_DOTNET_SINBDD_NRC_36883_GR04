using WS_CON_UNI_DOTNET_SOAP.Models.Enums;

namespace WS_CON_UNI_DOTNET_SOAP.Models.DTO
{
  public class CatalogResponseDTO : ResponseDTO
  {
    public HashSet<string> resultado { get; set; }

    public CatalogResponseDTO()
    {
      this.resultado = [];
    }

    public CatalogResponseDTO(EstadosEnum estado, string mensaje, HashSet<string> resultado) : base(estado, mensaje)
    {
      this.resultado = resultado;
    }
  }
}
