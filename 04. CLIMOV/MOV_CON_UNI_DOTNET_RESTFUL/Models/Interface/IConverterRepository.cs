using MOV_CON_UNI_DOTNET_RESTFUL.Models.data;

namespace MOV_CON_UNI_DOTNET_RESTFUL.Models.Interface
{
    public interface IConverterRepository
    {
        Task<ConvertResult> ConvertAsync(string token, Conversion conversion);
    }
}
