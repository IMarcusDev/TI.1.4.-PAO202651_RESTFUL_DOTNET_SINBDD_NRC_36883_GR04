using MOV_CON_UNI_DOTNET_RESTFUL.Models.data;
using MOV_CON_UNI_DOTNET_RESTFUL.Models.Enums;
using MOV_CON_UNI_DOTNET_RESTFUL.Models.Interface;
using MOV_CON_UNI_DOTNET_RESTFUL.Services;

namespace MOV_CON_UNI_DOTNET_RESTFUL.Controllers
{
    public class CatalogController(ICatalogRepository catalogRepository, SessionService session) : ControladorConSesion(session)
    {
        private readonly ICatalogRepository _catalogRepository = catalogRepository;

        public async Task<CatalogoCompletoResult> ObtenerCatalogoAsync()
        {
            if (!HaySesion)
                return new CatalogoCompletoResult(false, MensajeSesionExpirada, [], SesionExpirada: true);

            string token = Token!;
            Magnitud[] magnitudes = Enum.GetValues<Magnitud>();
            CatalogResult[] resultados = await Task.WhenAll(
                magnitudes.Select(m => _catalogRepository.GetCatalogAsync(token, m)));

            if (resultados.Any(r => r.SesionExpirada))
            {
                LimpiarSiExpiro(true);
                return new CatalogoCompletoResult(false, MensajeSesionExpirada, [], SesionExpirada: true);
            }

            CatalogResult? fallo = resultados.FirstOrDefault(r => !r.Success);
            if (fallo is not null)
                return new CatalogoCompletoResult(false, fallo.Message, []);

            List<GrupoUnidades> grupos = magnitudes
                .Zip(resultados, (magnitud, resultado) => new GrupoUnidades(
                    magnitud,
                    resultado.Unidades.Select(u => new UnidadItem(u, NombresUnidades.Nombre(u)))))
                .ToList();

            return new CatalogoCompletoResult(true, "", grupos);
        }
    }
}
