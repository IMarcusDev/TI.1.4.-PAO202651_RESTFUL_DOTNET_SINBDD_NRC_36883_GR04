using MOV_CON_UNI_DOTNET_RESTFUL.Controllers;
using MOV_CON_UNI_DOTNET_RESTFUL.Models.data;

namespace MOV_CON_UNI_DOTNET_RESTFUL.Views;

public partial class CatalogPage : ContentPage
{
    private const string ErrorInesperado = "Ocurrió un error inesperado. Intente de nuevo.";
    private const double AltoFilaUnidad = 60;

    private readonly CatalogController _controller;

    private bool _ocupado;
    private bool _cargado;
    private bool _hojaAbierta;

    public CatalogPage(CatalogController controller)
    {
        InitializeComponent();
        _controller = controller;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            if (!_controller.HaySesion)
            {
                await NavegacionSesion.VolverAlLoginAsync(this, ControladorConSesion.MensajeSesionExpirada);
                return;
            }

            if (!_cargado)
                await CargarAsync(desdeGesto: false);
        }
        catch (Exception)
        {
            MostrarError(ErrorInesperado);
        }
    }

    private async void OnRefreshing(object? sender, EventArgs e)
    {
        try
        {
            await CargarAsync(desdeGesto: true);
        }
        catch (Exception)
        {
            MostrarError(ErrorInesperado);
        }
    }

    private async void OnReintentarClicked(object? sender, EventArgs e)
    {
        try
        {
            await CargarAsync(desdeGesto: false);
        }
        catch (Exception)
        {
            MostrarError(ErrorInesperado);
        }
    }

    private async Task CargarAsync(bool desdeGesto)
    {
        if (_ocupado)
        {
            CatalogoRefresh.IsRefreshing = false;
            return;
        }

        _ocupado = true;
        ErrorPanel.IsVisible = false;
        ReintentarButton.IsEnabled = false;
        MostrarCargando(!desdeGesto);

        try
        {
            CatalogoCompletoResult resultado = await _controller.ObtenerCatalogoAsync();

            if (resultado.SesionExpirada)
            {
                await NavegacionSesion.VolverAlLoginAsync(this, resultado.Message);
                return;
            }

            if (resultado.Success)
            {
                CatalogoView.ItemsSource = resultado.Grupos;
                _cargado = true;
            }
            else
            {
                MostrarError(resultado.Message);
            }
        }
        finally
        {
            _ocupado = false;
            CatalogoRefresh.IsRefreshing = false;
            ReintentarButton.IsEnabled = true;
            MostrarCargando(false);
        }
    }

    private async void OnCerrarSesionClicked(object? sender, EventArgs e)
    {
        try
        {
            if (!await DisplayAlertAsync("Cerrar sesión", "¿Desea cerrar la sesión?", "Sí", "No"))
                return;

            _controller.CerrarSesion();
            await NavegacionSesion.VolverAlLoginAsync(this);
        }
        catch (Exception)
        {
            MostrarError(ErrorInesperado);
        }
    }

    private async void OnMagnitudTapped(object? sender, TappedEventArgs e)
    {
        try
        {
            if ((sender as BindableObject)?.BindingContext is GrupoUnidades grupo)
                await AbrirHojaAsync(grupo);
        }
        catch (Exception)
        {
            MostrarError(ErrorInesperado);
        }
    }

    private async Task AbrirHojaAsync(GrupoUnidades grupo)
    {
        if (_hojaAbierta)
            return;
        _hojaAbierta = true;

        HojaIcono.Source = grupo.Icono;
        HojaTitulo.Text = grupo.Nombre;
        HojaDescripcion.Text = grupo.Descripcion;
        HojaCantidad.Text = $"{grupo.Count} unidades disponibles";
        HojaUnidadesView.ItemsSource = grupo;
        HojaUnidadesView.HeightRequest = Math.Min(grupo.Count * AltoFilaUnidad, Height * 0.5);

        Hoja.TranslationY = Height;
        HojaFondo.Opacity = 0;
        HojaOverlay.IsVisible = true;
        await Task.WhenAll(
            Hoja.TranslateToAsync(0, 0, 260, Easing.CubicOut),
            HojaFondo.FadeToAsync(0.45, 260));
    }

    private async Task CerrarHojaAsync()
    {
        if (!_hojaAbierta)
            return;

        await Task.WhenAll(
            Hoja.TranslateToAsync(0, Hoja.Height, 200, Easing.CubicIn),
            HojaFondo.FadeToAsync(0, 200));
        HojaOverlay.IsVisible = false;
        _hojaAbierta = false;
    }

    private async void CerrarHoja()
    {
        try
        {
            await CerrarHojaAsync();
        }
        catch (Exception)
        {
            HojaOverlay.IsVisible = false;
            _hojaAbierta = false;
        }
    }

    private void OnCerrarHojaTapped(object? sender, TappedEventArgs e) => CerrarHoja();

    private void OnCerrarHojaClicked(object? sender, EventArgs e) => CerrarHoja();

    private void OnHojaDeslizada(object? sender, SwipedEventArgs e) => CerrarHoja();

    protected override bool OnBackButtonPressed()
    {
        if (!_hojaAbierta)
            return base.OnBackButtonPressed();

        CerrarHoja();
        return true;
    }

    private void MostrarCargando(bool cargando)
    {
        CargandoPanel.IsVisible = cargando;
        CargandoIndicator.IsRunning = cargando;
    }

    private void MostrarError(string mensaje)
    {
        ErrorLabel.Text = mensaje;
        ErrorPanel.IsVisible = true;
    }
}
