using MOV_CON_UNI_DOTNET_RESTFUL.Models.data;
using MOV_CON_UNI_DOTNET_RESTFUL.Models.Enums;
using MOV_CON_UNI_DOTNET_RESTFUL.Models.Interface;
using MOV_CON_UNI_DOTNET_RESTFUL.Services;

namespace MOV_CON_UNI_DOTNET_RESTFUL.Controllers
{
    public class ConversionController(
        IConverterRepository converterRepository,
        ICatalogRepository catalogRepository,
        SessionService session) : ControladorConSesion(session)
    {
        private readonly IConverterRepository _converterRepository = converterRepository;
        private readonly ICatalogRepository _catalogRepository = catalogRepository;

        public IReadOnlyList<MagnitudOpcion> Magnitudes { get; } =
            Enum.GetValues<Magnitud>().Select(m => new MagnitudOpcion(m, m.Nombre())).ToList();

        public async Task<CatalogResult> ObtenerUnidadesAsync(Magnitud? magnitud)
        {
            if (magnitud is null)
                return new CatalogResult(false, "Seleccione qué desea convertir", []);
            if (!HaySesion)
                return new CatalogResult(false, MensajeSesionExpirada, [], SesionExpirada: true);

            CatalogResult resultado = await _catalogRepository.GetCatalogAsync(Token!, magnitud.Value);

            LimpiarSiExpiro(resultado.SesionExpirada);
            return resultado.SesionExpirada ? resultado with { Message = MensajeSesionExpirada } : resultado;
        }

        public string FiltrarValor(string? texto, Magnitud? magnitud)
            => ValidadorConversion.FiltrarValor(texto, magnitud?.PermiteNegativos() ?? false);

        public async Task<ConvertResult> ConvertAsync(Magnitud? magnitud, string? valor, string? origen, string? destino)
        {
            ValidacionResult validacion = ValidadorConversion.Validar(magnitud, valor, origen, destino);
            if (!validacion.EsValido)
                return new ConvertResult(false, validacion.Mensaje, 0);

            var conversion = new Conversion(magnitud!.Value, validacion.Valor, origen!, destino!);

            if (origen == destino)
            {
                conversion.Result = validacion.Valor;
                return new ConvertResult(true, "Origen y destino son la misma unidad; el valor no cambia.",
                    validacion.Valor, Conversion: conversion);
            }

            if (!HaySesion)
                return new ConvertResult(false, MensajeSesionExpirada, 0, SesionExpirada: true);

            ConvertResult resultado = await _converterRepository.ConvertAsync(Token!, conversion);

            LimpiarSiExpiro(resultado.SesionExpirada);
            if (resultado.SesionExpirada)
                return resultado with { Message = MensajeSesionExpirada };
            if (!resultado.Success)
                return resultado;

            conversion.Result = resultado.Value;
            return resultado with { Conversion = conversion };
        }
    }
}
