using MOV_CON_UNI_DOTNET_RESTFUL.Controllers;

namespace MOV_CON_UNI_DOTNET_RESTFUL.Views;

public partial class LoginPage : ContentPage
{
    private readonly LoginController _controller;

    public LoginPage(LoginController controller)
    {
        InitializeComponent();
        _controller = controller;
    }

    private void OnUsuarioCompleted(object? sender, EventArgs e) => PasswordEntry.Focus();

    private async void OnIngresarClicked(object? sender, EventArgs e)
    {
        if (!IngresarButton.IsEnabled)
            return;

        MostrarCargando(true);
        ErrorLabel.IsVisible = false;

        try
        {
            var resultado = await _controller.LoginAsync(UsernameEntry.Text, PasswordEntry.Text);

            if (resultado.Success)
            {
                PasswordEntry.Text = string.Empty;
                await Shell.Current.GoToAsync("//main/conversion");
            }
            else
            {
                MostrarError(resultado.Message);
            }
        }
        catch (Exception)
        {
            MostrarError("Ocurrió un error inesperado. Intente de nuevo.");
        }
        finally
        {
            MostrarCargando(false);
        }
    }

    private void MostrarCargando(bool cargando)
    {
        IngresarButton.IsEnabled = !cargando;
        UsernameEntry.IsEnabled = !cargando;
        PasswordEntry.IsEnabled = !cargando;
        CargandoIndicator.IsRunning = cargando;
        CargandoIndicator.IsVisible = cargando;
    }

    private void MostrarError(string mensaje)
    {
        ErrorLabel.Text = mensaje;
        ErrorLabel.IsVisible = true;
    }
}
