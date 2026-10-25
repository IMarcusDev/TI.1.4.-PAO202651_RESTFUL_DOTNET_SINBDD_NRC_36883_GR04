using MOV_CON_UNI_DOTNET_RESTFUL.Models.data;
using MOV_CON_UNI_DOTNET_RESTFUL.Models.DTO;
using MOV_CON_UNI_DOTNET_RESTFUL.Models.Enums;
using MOV_CON_UNI_DOTNET_RESTFUL.Models.Interface;
using MOV_CON_UNI_DOTNET_RESTFUL.Services;

namespace MOV_CON_UNI_DOTNET_RESTFUL.Models.Repositories
{
    public class CatalogRepository(ClienteRest cliente) : ICatalogRepository
    {
        private readonly ClienteRest _cliente = cliente;

        public async Task<CatalogResult> GetCatalogAsync(string token, Magnitud magnitud)
        {
            if (RestErrores.SinRed())
                return new CatalogResult(false, RestErrores.SinConexion, []);

            try
            {
                RespuestaHttp respuesta = await _cliente.GetAsync(
                    $"catalog/{Uri.EscapeDataString(magnitud.CodigoServidor())}", token);

                CatalogResponseDTO? dto = RestErrores.LeerDto(respuesta, JsonContexto.Default.CatalogResponseDTO);
                if (dto == null)
                    return new CatalogResult(false, RestErrores.MensajeSinDto(respuesta), []);

                if (dto.Estado != EstadosEnum.OK)
                    return new CatalogResult(
                        Success: false,
                        Message: RestErrores.MensajeServidor(dto.Mensaje, "No se pudo obtener el catálogo"),
                        Unidades: [],
                        SesionExpirada: RestErrores.EsTokenInvalido(dto.Mensaje)
                        );

                List<string> unidades = (dto.Resultado ?? [])
                    .Where(u => !string.IsNullOrWhiteSpace(u))
                    .Select(u => u.Trim())
                    .Distinct()
                    .ToList();

                if (unidades.Count == 0)
                    return new CatalogResult(false, $"El servidor no devolvió unidades para {magnitud.Nombre()}.", []);

                return new CatalogResult(true, dto.Mensaje ?? "", unidades);
            }
            catch (Exception ex)
            {
                return new CatalogResult(false, RestErrores.Mensaje(ex), []);
            }
        }
    }
}
