using ec.edu.monster.vista;

namespace ec.edu.monster.pruebas
{
  /// <summary>
  /// Prueba los controles WinForms reales sin mostrarlos en pantalla.
  /// </summary>
  public class PanelesTest
  {
    [Fact]
    public void ConversionMuestraNombresPeroDevuelveValoresDelServidor()
    {
      using var panel = new ConversionPanel();

      panel.MostrarMagnitudes(["Longitud", "Area"]);
      panel.MostrarUnidades(["m", "ft", "xyz"]);

      Assert.Equal("Longitud", panel.Magnitud);
      Assert.Equal("Área", panel.ComboCategoria.Items[1]!.ToString());
      Assert.Equal("Metro (m)", panel.ComboDesde.Items[0]!.ToString());
      Assert.Equal("xyz", panel.ComboDesde.Items[2]!.ToString());
      Assert.Equal("m", panel.UnidadOrigen);
      Assert.Equal("ft", panel.UnidadDestino);
    }

    [Fact]
    public void ConversionConUnaSolaUnidadUsaLaMismaEnAmbos()
    {
      using var panel = new ConversionPanel();

      panel.MostrarUnidades(["s"]);

      Assert.Equal("s", panel.UnidadOrigen);
      Assert.Equal("s", panel.UnidadDestino);
    }

    [Fact]
    public void ConversionSinUnidadesNoTieneSeleccion()
    {
      using var panel = new ConversionPanel();
      panel.MostrarUnidades(["m", "ft"]);

      panel.MostrarUnidades([]);

      Assert.Null(panel.UnidadOrigen);
      Assert.Null(panel.UnidadDestino);
    }

    [Fact]
    public void LlenarLasMagnitudesNoDisparaLaAccionDelUsuario()
    {
      using var panel = new ConversionPanel();
      int llamadas = 0;
      panel.AlCambiarMagnitud(() => { llamadas++; return Task.CompletedTask; });

      panel.MostrarMagnitudes(["Longitud", "Masa"]);

      Assert.Equal(0, llamadas);
    }

    [Fact]
    public void ElUsuarioCambiaLaMagnitudYSeAvisaAlControlador()
    {
      using var panel = new ConversionPanel();
      int llamadas = 0;
      panel.AlCambiarMagnitud(() => { llamadas++; return Task.CompletedTask; });
      panel.MostrarMagnitudes(["Longitud", "Masa"]);

      panel.ComboCategoria.SelectedIndex = 1;

      Assert.Equal(1, llamadas);
      Assert.Equal("Masa", panel.Magnitud);
    }

    [Fact]
    public void ConversionResultadoYErrorSeLimpian()
    {
      using var panel = new ConversionPanel();
      panel.MostrarResultado("1 m = 100,0000 cm");
      panel.MostrarError("Algo falló");

      panel.LimpiarMensajes();

      Assert.Equal("", panel.EtiquetaResultado.Text);
      Assert.Equal("", panel.EtiquetaError.Text);
    }

    [Fact]
    public void CatalogoMuestraNombreYSimboloDeCadaUnidad()
    {
      using var panel = new CatalogoPanel();

      panel.MostrarMagnitudes(["Temperatura", "Area"]);
      panel.MostrarUnidades(["°C", "K"]);

      Assert.Equal("Temperatura", panel.Magnitud);
      Assert.Equal(2, panel.TablaUnidades.Items.Count);
      Assert.Equal("Celsius", panel.TablaUnidades.Items[0].SubItems[0].Text);
      Assert.Equal("°C", panel.TablaUnidades.Items[0].SubItems[1].Text);
      Assert.Equal("Kelvin", panel.TablaUnidades.Items[1].SubItems[0].Text);
    }

    [Fact]
    public void CatalogoReemplazaLasUnidadesAnteriores()
    {
      using var panel = new CatalogoPanel();
      panel.MostrarUnidades(["m", "cm", "ft"]);

      panel.MostrarUnidades([]);

      Assert.Empty(panel.TablaUnidades.Items);
    }

    [Fact]
    public void CatalogoAvisaAlControladorCuandoElUsuarioEligeOtraMagnitud()
    {
      using var panel = new CatalogoPanel();
      int llamadas = 0;
      panel.AlCambiarMagnitud(() => { llamadas++; return Task.CompletedTask; });
      panel.MostrarMagnitudes(["Longitud", "Masa"]);
      Assert.Equal(0, llamadas);

      panel.ListaMagnitudes.SelectedIndex = 1;

      Assert.Equal(1, llamadas);
      Assert.Equal("Masa", panel.Magnitud);
    }

    [Fact]
    public void ElIndicadorEmpiezaComprobandoYCambiaDeColor()
    {
      using var indicador = new IndicadorEstado("Servicio disponible", "Servicio no disponible");
      Assert.Equal(IndicadorEstado.TextoComprobando, indicador.Text);

      indicador.MostrarEstado(false);
      Assert.Equal("Servicio no disponible", indicador.Text);
      Assert.False(indicador.Disponible);

      indicador.MostrarEstado(true);
      Assert.Equal("Servicio disponible", indicador.Text);
      Assert.True(indicador.Disponible);
    }

    [Fact]
    public void ElMenuDeLaVentanaPrincipalCambiaDePantalla()
    {
      using var principal = new PrincipalFrame(() => { });
      Assert.Same(principal.ConversionPanel, principal.PantallaActual);

      principal.OpcionArchivo("Consultar catálogo").PerformClick();
      Assert.Same(principal.CatalogoPanel, principal.PantallaActual);

      principal.OpcionArchivo("Convertir unidades").PerformClick();
      Assert.Same(principal.ConversionPanel, principal.PantallaActual);
    }

    [Fact]
    public void CerrarSesionAvisaAApp()
    {
      bool cerrada = false;
      using var principal = new PrincipalFrame(() => cerrada = true);

      principal.OpcionArchivo("Cerrar sesión").PerformClick();

      Assert.True(cerrada);
    }
  }
}
