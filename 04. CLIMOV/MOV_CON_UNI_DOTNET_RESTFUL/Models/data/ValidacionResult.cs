namespace MOV_CON_UNI_DOTNET_RESTFUL.Models.data
{
    public record ValidacionResult(bool EsValido, string Mensaje, double Valor = 0)
    {
        public static ValidacionResult Error(string mensaje) => new(false, mensaje);
        public static ValidacionResult Ok(double valor) => new(true, "", valor);
    }
}
