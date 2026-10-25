
using MOV_CON_UNI_DOTNET_RESTFUL.Models.data;

namespace MOV_CON_UNI_DOTNET_RESTFUL.Models.Interface
{
    public interface ILoginRepository
    {
        Task<LoginResult> LoginAsync(string username, string password);
    }
}
