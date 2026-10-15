namespace WS_CON_UNI_DOTNET_RESTful.Models.DTO
{
  public class ConvertRequestDTO
  {
    public string? type { get; set; }
    public double value { get; set; }
    public string? from { get; set; }
    public string? to { get; set; }

    public ConvertRequestDTO() { }

    public ConvertRequestDTO(string type, double value, string from, string to)
    {
      this.type = type;
      this.value = value;
      this.from = from;
      this.to = to;
    }
  }
}
