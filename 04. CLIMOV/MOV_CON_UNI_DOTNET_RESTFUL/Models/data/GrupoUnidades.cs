using MOV_CON_UNI_DOTNET_RESTFUL.Models.Enums;

namespace MOV_CON_UNI_DOTNET_RESTFUL.Models.data
{
    public class GrupoUnidades(Magnitud magnitud, IEnumerable<UnidadItem> unidades) : List<UnidadItem>(unidades)
    {
        public Magnitud Magnitud { get; } = magnitud;
        public string Nombre { get; } = magnitud.Nombre();
        public string Descripcion { get; } = magnitud.Descripcion();
        public string Icono { get; } = magnitud.Icono();
    }
}
