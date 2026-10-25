namespace MOV_CON_UNI_DOTNET_RESTFUL.Models.data
{
    public record CatalogoCompletoResult(bool Success, string Message, IReadOnlyList<GrupoUnidades> Grupos, bool SesionExpirada = false);
}
