namespace MOV_CON_UNI_DOTNET_RESTFUL.Views;

internal static class NavegacionSesion
{
    public static async Task VolverAlLoginAsync(Page pagina, string? aviso = null)
    {
        if (aviso is not null)
            await pagina.DisplayAlertAsync("Sesión", aviso, "Aceptar");

        if (Application.Current?.Windows.FirstOrDefault() is Window ventana)
            ventana.Page = new AppShell();
        else
            await Shell.Current.GoToAsync("//login");
    }
}
