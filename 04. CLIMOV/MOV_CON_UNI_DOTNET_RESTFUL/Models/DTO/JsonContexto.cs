using System.Text.Json;
using System.Text.Json.Serialization;

namespace MOV_CON_UNI_DOTNET_RESTFUL.Models.DTO
{
    [JsonSourceGenerationOptions(JsonSerializerDefaults.Web, UseStringEnumConverter = true)]
    [JsonSerializable(typeof(LoginRequestDTO))]
    [JsonSerializable(typeof(LoginResponseDTO))]
    [JsonSerializable(typeof(CatalogResponseDTO))]
    [JsonSerializable(typeof(ConvertRequestDTO))]
    [JsonSerializable(typeof(ConverterResponseDTO))]
    internal partial class JsonContexto : JsonSerializerContext
    {
    }
}
