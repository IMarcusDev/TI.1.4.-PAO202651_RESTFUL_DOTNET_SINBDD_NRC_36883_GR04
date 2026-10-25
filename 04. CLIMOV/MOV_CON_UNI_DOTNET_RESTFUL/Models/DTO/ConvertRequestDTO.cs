namespace MOV_CON_UNI_DOTNET_RESTFUL.Models.DTO
{
    public class ConvertRequestDTO(string type, double value, string from, string to)
    {
        public string Type { get; } = type;
        public double Value { get; } = value;
        public string From { get; } = from;
        public string To { get; } = to;
    }
}
