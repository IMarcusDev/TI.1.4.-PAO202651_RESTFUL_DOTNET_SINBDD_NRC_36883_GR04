namespace MOV_CON_UNI_DOTNET_RESTFUL.Models.Repositories
{
    internal static partial class RestErrores
    {
        public static bool SinRed()
        {
            try
            {
                return Connectivity.Current.NetworkAccess == NetworkAccess.None;
            }
            catch
            {
                return false;
            }
        }
    }
}
