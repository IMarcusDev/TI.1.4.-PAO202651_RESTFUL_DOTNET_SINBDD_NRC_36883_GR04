using MOV_CON_UNI_DOTNET_RESTFUL.Controllers;
using MOV_CON_UNI_DOTNET_RESTFUL.Models.data;
using MOV_CON_UNI_DOTNET_RESTFUL.Models.Enums;
using MOV_CON_UNI_DOTNET_RESTFUL.Utils;

namespace MOV_CON_UNI_DOTNET_RESTFUL.Views;

public partial class ConversionPage : ContentPage
{
    private const string DetalleInicial = "Ingrese un valor y pulse Convertir.";
    private const string ErrorInesperado = "Ocurrió un error inesperado. Intente de nuevo.";

    private readonly ConversionController _controller;

    private bool _ocupado;
    private bool _filtrando;
    private int _solicitudUnidades;

    public ConversionPage(ConversionController controller)
    {
        InitializeComponent();
        _controller = controller;
        MagnitudesView.ItemsSource = _controller.Magnitudes;
    }

    private Magnitud? MagnitudSeleccionada => (MagnitudesView.SelectedItem as MagnitudOpcion)?.Valor;

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

            if (MagnitudesView.SelectedItem is null && _controller.Magnitudes.Count > 0)
                MagnitudesView.SelectedItem = _controller.Magnitudes[0];
        }
        catch (Exception)
        {
            MostrarError(ErrorInesperado);
        }
    }

    private async void OnMagnitudChanged(object? sender, SelectionChangedEventArgs e)
    {
        try
        {
            MagnitudActualSpan.Text = (MagnitudesView.SelectedItem as MagnitudOpcion)?.Nombre ?? "—";

            OcultarError();
            LimpiarUnidades();
            LimpiarResultado();
            await CargarUnidadesAsync();
        }
        catch (Exception)
        {
            MostrarError(ErrorInesperado);
        }
    }

    private async void OnReintentarUnidadesClicked(object? sender, EventArgs e)
    {
        try
        {
            OcultarError();
            await CargarUnidadesAsync();
        }
        catch (Exception)
        {
            MostrarError(ErrorInesperado);
        }
    }

    private async Task CargarUnidadesAsync()
    {
        Magnitud? magnitud = MagnitudSeleccionada;
        if (magnitud is null)
            return;

        int solicitud = ++_solicitudUnidades;
        EstablecerOcupado(true);
        DetalleLabel.Text = "Cargando unidades…";

        try
        {
            CatalogResult resultado = await _controller.ObtenerUnidadesAsync(magnitud);

            if (solicitud != _solicitudUnidades)
                return;

            if (resultado.SesionExpirada)
            {
                await NavegacionSesion.VolverAlLoginAsync(this, resultado.Message);
                return;
            }

            if (!resultado.Success)
            {
                MostrarError(resultado.Message);
                ReintentarUnidadesButton.IsVisible = true;
                DetalleLabel.Text = "No se pudieron cargar las unidades.";
                return;
            }

            OrigenPicker.ItemsSource = resultado.Unidades.ToList();
            DestinoPicker.ItemsSource = resultado.Unidades.ToList();
            DetalleLabel.Text = DetalleInicial;
        }
        finally
        {
            if (solicitud == _solicitudUnidades)
                EstablecerOcupado(false);
        }
    }

    private void OnValorTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_filtrando)
            return;

        try
        {
            string original = e.NewTextValue ?? "";
            string filtrado = _controller.FiltrarValor(original, MagnitudSeleccionada);

            if (filtrado != original)
            {
                _filtrando = true;
                ValorEntry.Text = filtrado;
                ValorEntry.CursorPosition = filtrado.Length;
            }

            if (filtrado != (e.OldTextValue ?? ""))
                LimpiarResultado();
        }
        catch (Exception)
        {
        }
        finally
        {
            _filtrando = false;
        }
    }

    private void OnUnidadChanged(object? sender, EventArgs e)
    {
        OcultarError();
        LimpiarResultado();
    }

    private void OnInvertirClicked(object? sender, EventArgs e)
    {
        if (_ocupado)
            return;

        object? origen = OrigenPicker.SelectedItem;
        OrigenPicker.SelectedItem = DestinoPicker.SelectedItem;
        DestinoPicker.SelectedItem = origen;
    }

    private async void OnConvertirClicked(object? sender, EventArgs e)
    {
        if (_ocupado)
            return;

        ValorEntry.Unfocus();
        OcultarError();
        EstablecerOcupado(true);

        try
        {
            ConvertResult resultado = await _controller.ConvertAsync(
                MagnitudSeleccionada,
                ValorEntry.Text,
                OrigenPicker.SelectedItem as string,
                DestinoPicker.SelectedItem as string);

            if (resultado.SesionExpirada)
            {
                await NavegacionSesion.VolverAlLoginAsync(this, resultado.Message);
                return;
            }

            if (resultado.Success)
                MostrarResultado(resultado);
            else
            {
                LimpiarResultado();
                MostrarError(resultado.Message);
            }
        }
        catch (Exception)
        {
            LimpiarResultado();
            MostrarError(ErrorInesperado);
        }
        finally
        {
            EstablecerOcupado(false);
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

    private void MostrarResultado(ConvertResult resultado)
    {
        string texto = FormatoNumero.Formatear(resultado.Value);
        ResultadoLabel.Text = texto;
        ResultadoLabel.FontSize = texto.Length > 12 ? 30 : 44;

        if (resultado.Conversion is Conversion c)
        {
            DetalleLabel.Text = $"{FormatoNumero.Formatear(c.Value)} {c.From} = {texto} {c.To}";
            if (c.From == c.To)
                DetalleLabel.Text += Environment.NewLine + resultado.Message;
        }
    }

    private void LimpiarResultado()
    {
        ResultadoLabel.Text = "—";
        ResultadoLabel.FontSize = 44;
        DetalleLabel.Text = DetalleInicial;
    }

    private void LimpiarUnidades()
    {
        OrigenPicker.ItemsSource = null;
        DestinoPicker.ItemsSource = null;
    }

    private void EstablecerOcupado(bool ocupado)
    {
        _ocupado = ocupado;
        ConvertirButton.IsEnabled = !ocupado;
        InvertirButton.IsEnabled = !ocupado;
        ReintentarUnidadesButton.IsEnabled = !ocupado;
        MagnitudesView.InputTransparent = ocupado;
        ValorEntry.IsEnabled = !ocupado;
        OrigenPicker.IsEnabled = !ocupado;
        DestinoPicker.IsEnabled = !ocupado;
        CargandoIndicator.IsRunning = ocupado;
        CargandoIndicator.IsVisible = ocupado;
    }

    private void MostrarError(string mensaje)
    {
        ErrorLabel.Text = mensaje;
        ErrorLabel.IsVisible = true;
    }

    private void OcultarError()
    {
        ErrorLabel.IsVisible = false;
        ReintentarUnidadesButton.IsVisible = false;
    }
}
