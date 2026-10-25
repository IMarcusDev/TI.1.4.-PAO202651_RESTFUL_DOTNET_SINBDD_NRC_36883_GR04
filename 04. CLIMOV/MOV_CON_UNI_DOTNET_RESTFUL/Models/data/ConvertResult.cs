namespace MOV_CON_UNI_DOTNET_RESTFUL.Models.data
{
    public record ConvertResult(bool Success, string Message, double Value, bool SesionExpirada = false, Conversion? Conversion = null);
}
