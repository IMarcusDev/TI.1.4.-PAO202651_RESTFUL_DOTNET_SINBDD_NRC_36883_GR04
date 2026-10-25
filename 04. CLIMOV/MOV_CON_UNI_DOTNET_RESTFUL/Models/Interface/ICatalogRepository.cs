using MOV_CON_UNI_DOTNET_RESTFUL.Models.data;
using MOV_CON_UNI_DOTNET_RESTFUL.Models.Enums;

namespace MOV_CON_UNI_DOTNET_RESTFUL.Models.Interface
{
    public interface ICatalogRepository
    {
        Task<CatalogResult> GetCatalogAsync(string token, Magnitud magnitud);
    }
}
