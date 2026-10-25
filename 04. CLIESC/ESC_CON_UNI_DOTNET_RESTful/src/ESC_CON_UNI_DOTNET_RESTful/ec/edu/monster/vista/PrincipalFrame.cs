namespace ec.edu.monster.vista
{
  /// <summary>
  /// Ventana principal tras el login. El menú "Archivo" cambia entre conversión y
  /// catálogo y cierra la sesión. App conecta los controladores con los paneles.
  /// </summary>
  public class PrincipalFrame : Form
  {
    public ConversionPanel ConversionPanel { get; } = new();
    public CatalogoPanel CatalogoPanel { get; } = new();

    public IndicadorEstado Estado { get; } =
      new("Sesión activa · Servicio disponible", "Sesión activa · Servidor no disponible");

    public PrincipalFrame(Action alCerrarSesion)
    {
      SuspendLayout();
      AutoScaleDimensions = new SizeF(96F, 96F);
      AutoScaleMode = AutoScaleMode.Dpi;
      Text = Estilos.TituloVentana;
      ClientSize = new Size(820, 560);
      MinimumSize = new Size(700, 480);
      StartPosition = FormStartPosition.CenterScreen;
      BackColor = Estilos.Fondo;
      Font = Estilos.Fuente;

      var contenido = new Panel { Dock = DockStyle.Fill };
      contenido.Controls.Add(ConversionPanel);
      contenido.Controls.Add(CatalogoPanel);
      Mostrar(ConversionPanel);

      Controls.Add(contenido);
      Controls.Add(Estado);
      MainMenuStrip = Estilos.BarraMenu(
        Estilos.Menu("Archivo",
          Estilos.Opcion("Convertir unidades", () => Mostrar(ConversionPanel)),
          Estilos.Opcion("Consultar catálogo", () => Mostrar(CatalogoPanel)),
          new ToolStripSeparator(),
          Estilos.Opcion("Cerrar sesión", () =>
          {
            try
            {
              alCerrarSesion();
            }
            finally
            {
              Close();
            }
          }),
          Estilos.Opcion("Salir", Application.Exit)),
        Estilos.MenuAyuda(this));
      Controls.Add(MainMenuStrip);

      Shown += (_, _) => Estado.Iniciar();
      ResumeLayout(false);
      PerformLayout();
    }

    private void Mostrar(Control pantalla)
    {
      PantallaActual = pantalla;
      ConversionPanel.Visible = pantalla == ConversionPanel;
      CatalogoPanel.Visible = pantalla == CatalogoPanel;
    }

    /// <summary>Pantalla elegida en el menú; las pruebas la usan para revisar la navegación.</summary>
    internal Control? PantallaActual { get; private set; }

    /// <summary>Opción del menú "Archivo" con ese texto, para simular clics en las pruebas.</summary>
    internal ToolStripItem OpcionArchivo(string texto) =>
      ((ToolStripMenuItem)MainMenuStrip!.Items[0]).DropDownItems.Cast<ToolStripItem>().First(o => o.Text == texto);
  }
}
