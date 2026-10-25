namespace ec.edu.monster.vista
{
  /// <summary>
  /// Ventana de login. La navegación a la ventana principal se delega en el
  /// callback alAbrirMenu que entrega App.
  /// </summary>
  public class LoginFrame : Form, ILoginVista
  {
    private readonly Action alAbrirMenu;
    private readonly TextBox campoUsuario = Estilos.Entrada();
    private readonly TextBox campoClave = Estilos.Entrada(clave: true);
    private readonly Button botonIngresar = Estilos.BotonPrimario("Ingresar");
    private Func<Task> accionIngresar = () => Task.CompletedTask;

    public IndicadorEstado Estado { get; } =
      new("Servicio disponible", "Servicio no disponible");

    public LoginFrame(Action alAbrirMenu)
    {
      this.alAbrirMenu = alAbrirMenu;

      SuspendLayout();
      AutoScaleDimensions = new SizeF(96F, 96F);
      AutoScaleMode = AutoScaleMode.Dpi;
      Text = Estilos.TituloVentana;
      ClientSize = new Size(640, 460);
      MinimumSize = new Size(480, 420);
      StartPosition = FormStartPosition.CenterScreen;
      BackColor = Estilos.Fondo;
      Font = Estilos.Fuente;

      Controls.Add(ConstruirContenido());
      Controls.Add(Estado);
      MainMenuStrip = Estilos.BarraMenu(
        Estilos.Menu("Archivo", Estilos.Opcion("Salir", Close)),
        Estilos.MenuAyuda(this));
      Controls.Add(MainMenuStrip);

      AcceptButton = botonIngresar;
      botonIngresar.Click += (_, _) => Ingresar();
      Shown += (_, _) =>
      {
        campoUsuario.Focus();
        Estado.Iniciar();
      };
      ResumeLayout(false);
      PerformLayout();
    }

    private Control ConstruirContenido()
    {
      var formulario = new TableLayoutPanel
      {
        ColumnCount = 1,
        Dock = DockStyle.Fill,
        Padding = new Padding(50, 30, 50, 0),
        BackColor = Estilos.Fondo,
      };
      formulario.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

      var filaBoton = new FlowLayoutPanel
      {
        FlowDirection = FlowDirection.RightToLeft,
        Dock = DockStyle.Fill,
        AutoSize = true,
        Margin = new Padding(0),
      };
      filaBoton.Controls.Add(botonIngresar);

      formulario.Controls.Add(Estilos.Titulo("Iniciar sesión"));
      formulario.Controls.Add(Estilos.Etiqueta("Usuario"));
      formulario.Controls.Add(campoUsuario);
      formulario.Controls.Add(Estilos.Etiqueta("Contraseña"));
      formulario.Controls.Add(campoClave);
      formulario.Controls.Add(filaBoton);
      return formulario;
    }

    private void Ingresar()
    {
      Estilos.Ejecutar(accionIngresar, MostrarError, botonIngresar, campoUsuario, campoClave);
    }

    public string Usuario => campoUsuario.Text;

    public string Clave => campoClave.Text;

    public void MostrarError(string mensaje)
    {
      MessageBox.Show(this, mensaje, Estilos.TituloVentana, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }

    public void AbrirMenu()
    {
      // La contraseña no se queda en memoria de una ventana que ya no se usa.
      campoClave.Clear();
      alAbrirMenu();
      Close();
    }

    public void AlIngresar(Func<Task> accion)
    {
      accionIngresar = accion;
    }
  }
}
