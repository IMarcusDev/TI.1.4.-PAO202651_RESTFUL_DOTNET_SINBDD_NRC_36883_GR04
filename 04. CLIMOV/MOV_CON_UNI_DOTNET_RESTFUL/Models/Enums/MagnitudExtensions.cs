namespace MOV_CON_UNI_DOTNET_RESTFUL.Models.Enums
{
    public static class MagnitudExtensions
    {
        public static string CodigoServidor(this Magnitud magnitud) => magnitud switch
        {
            Magnitud.Area => "Area",
            Magnitud.Longitud => "Longitud",
            Magnitud.Masa => "Masa",
            Magnitud.Temperatura => "Temperatura",
            Magnitud.Volumen => "Volumen",
            Magnitud.Tiempo => "Tiempo",
            _ => throw new ArgumentOutOfRangeException(nameof(magnitud), magnitud, null),
        };

        public static string Nombre(this Magnitud magnitud) => magnitud switch
        {
            Magnitud.Area => "Área",
            Magnitud.Longitud => "Longitud",
            Magnitud.Masa => "Masa",
            Magnitud.Temperatura => "Temperatura",
            Magnitud.Volumen => "Volumen",
            Magnitud.Tiempo => "Tiempo",
            _ => magnitud.ToString(),
        };

        public static string Descripcion(this Magnitud magnitud) => magnitud switch
        {
            Magnitud.Area => "Extensión de una superficie.",
            Magnitud.Longitud => "Distancia entre dos puntos.",
            Magnitud.Masa => "Cantidad de materia de un cuerpo.",
            Magnitud.Temperatura => "Nivel de calor de un cuerpo o ambiente.",
            Magnitud.Volumen => "Espacio que ocupa un cuerpo.",
            Magnitud.Tiempo => "Duración de un suceso.",
            _ => "",
        };

        public static string Icono(this Magnitud magnitud) => magnitud switch
        {
            Magnitud.Area => "mag_area.png",
            Magnitud.Longitud => "mag_longitud.png",
            Magnitud.Masa => "mag_masa.png",
            Magnitud.Temperatura => "mag_temperatura.png",
            Magnitud.Volumen => "mag_volumen.png",
            Magnitud.Tiempo => "mag_tiempo.png",
            _ => "",
        };

        public static bool PermiteNegativos(this Magnitud magnitud) => magnitud == Magnitud.Temperatura;
    }
}
