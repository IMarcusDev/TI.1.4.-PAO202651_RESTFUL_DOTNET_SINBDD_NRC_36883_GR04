using MOV_CON_UNI_DOTNET_RESTFUL.Services;

namespace MOV_CON_UNI_DOTNET_RESTFUL.Controllers
{
    public abstract class ControladorConSesion(SessionService session)
    {
        public const string MensajeSesionExpirada = "Su sesión expiró o no es válida. Inicie sesión nuevamente.";

        protected SessionService Session { get; } = session;

        protected string? Token => Session.Token;

        public bool HaySesion => Session.IsAuthenticated;

        public void CerrarSesion() => Session.Clear();

        protected void LimpiarSiExpiro(bool sesionExpirada)
        {
            if (sesionExpirada)
                Session.Clear();
        }
    }
}
