using MOV_CON_UNI_DOTNET_RESTFUL.Models.data;
using MOV_CON_UNI_DOTNET_RESTFUL.Models.DTO;
using MOV_CON_UNI_DOTNET_RESTFUL.Models.Enums;
using MOV_CON_UNI_DOTNET_RESTFUL.Models.Interface;
using MOV_CON_UNI_DOTNET_RESTFUL.Services;
using System.Text.Json;

namespace MOV_CON_UNI_DOTNET_RESTFUL.Models.Repositories
{
    public class ConvertRepository(ClienteRest cliente) : IConverterRepository
    {
        private readonly ClienteRest _cliente = cliente;

        public async Task<ConvertResult> ConvertAsync(string token, Conversion conversion)
        {
            if (RestErrores.SinRed())
                return new ConvertResult(false, RestErrores.SinConexion, 0);

            try
            {
                string json = JsonSerializer.Serialize(
                    new ConvertRequestDTO(
                        conversion.Magnitude.CodigoServidor(),
                        conversion.Value,
                        conversion.From,
                        conversion.To),
                    JsonContexto.Default.ConvertRequestDTO);

                RespuestaHttp respuesta = await _cliente.PostJsonAsync("converter", json, token);

                ConverterResponseDTO? dto = RestErrores.LeerDto(respuesta, JsonContexto.Default.ConverterResponseDTO);
                if (dto == null)
                    return new ConvertResult(false, RestErrores.MensajeSinDto(respuesta), 0);

                if (dto.Estado != EstadosEnum.OK)
                    return new ConvertResult(
                        Success: false,
                        Message: RestErrores.MensajeServidor(dto.Mensaje, "No se pudo hacer la conversión"),
                        Value: 0,
                        SesionExpirada: RestErrores.EsTokenInvalido(dto.Mensaje)
                        );

                if (dto.Resultado is not double resultado)
                    return new ConvertResult(false, RestErrores.RespuestaIncompleta, 0);

                if (!double.IsFinite(resultado))
                    return new ConvertResult(false, RestErrores.ResultadoNoValido, 0);

                return new ConvertResult(
                    Success: true,
                    Message: dto.Mensaje ?? "",
                    Value: resultado
                    );
            }
            catch (Exception ex)
            {
                return new ConvertResult(false, RestErrores.Mensaje(ex), 0);
            }
        }
    }
}
