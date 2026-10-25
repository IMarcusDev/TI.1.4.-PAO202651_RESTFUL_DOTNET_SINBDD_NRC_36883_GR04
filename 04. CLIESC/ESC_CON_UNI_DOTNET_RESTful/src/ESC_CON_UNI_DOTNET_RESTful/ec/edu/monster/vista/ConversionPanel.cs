using ec.edu.monster.modelo;

namespace ec.edu.monster.vista
{
  /// <summary>
  /// Pantalla "Convertir unidades": magnitud y unidades, valor y resultado. Los combos
  /// guardan el valor del servidor y muestran el nombre en español.
  /// </summary>
  public class ConversionPanel : UserControl, IConversionVista
  {
    private readonly ComboBox comboCategoria = Estilos.Combo();
    private readonly ComboBox comboDesde = Estilos.Combo();
    private readonly ComboBox comboHacia = Estilos.Combo();
    private readonly TextBox campoValor = Estilos.Entrada();
    private readonly Button botonConvertir = Estilos.BotonPrimario("Convertir");
    private readonly Label etiquetaResultado = new()
    {
      Text = "",
      Font = Estilos.FuenteResultado,
      ForeColor = Estilos.Texto,
      AutoSize = true,
    };
    private readonly Label etiquetaError = Estilos.MensajeError();

    private Func<Task> accionConvertir = () => Task.CompletedTask;
    private Func<Task> accionCambiarMagnitud = () => Task.CompletedTask;
    // True mientras los combos se llenan desde el código, para no disparar acciones del usuario.
    private bool actualizando;

    public ConversionPanel()
    {
      BackColor = Estilos.Fondo;
      Font = Estilos.Fuente;
      Dock = DockStyle.Fill;
      Padding = new Padding(34, 24, 34, 8);
      Controls.Add(ConstruirContenido());

      comboCategoria.SelectedIndexChanged += (_, _) => CambiarMagnitud();
      botonConvertir.Click += (_, _) => Convertir();
      campoValor.KeyDown += (_, e) =>
      {
        if (e.KeyCode == Keys.Enter)
        {
          e.SuppressKeyPress = true;
          Convertir();
        }
      };
    }

    private Control ConstruirContenido()
    {
      var contenido = new TableLayoutPanel { ColumnCount = 1, Dock = DockStyle.Top, AutoSize = true };
      contenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

      var selectores = new TableLayoutPanel { ColumnCount = 3, Dock = DockStyle.Fill, AutoSize = true, Margin = new Padding(0, 0, 0, 14) };
      for (int i = 0; i < 3; i++)
        selectores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
      selectores.Controls.Add(Campo("Categoría", comboCategoria), 0, 0);
      selectores.Controls.Add(Campo("Desde", comboDesde), 1, 0);
      selectores.Controls.Add(Campo("Hacia", comboHacia), 2, 0);

      var filaValor = new TableLayoutPanel { ColumnCount = 2, Dock = DockStyle.Fill, AutoSize = true, Margin = new Padding(0, 0, 0, 16) };
      filaValor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
      filaValor.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
      campoValor.Margin = new Padding(0);
      filaValor.Controls.Add(Campo("Valor", campoValor), 0, 0);
      botonConvertir.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
      botonConvertir.Margin = new Padding(12, 0, 0, 0);
      filaValor.Controls.Add(botonConvertir, 1, 0);

      contenido.Controls.Add(Estilos.Titulo("Convertir unidades"));
      contenido.Controls.Add(selectores);
      contenido.Controls.Add(filaValor);
      contenido.Controls.Add(ConstruirResultado());
      contenido.Controls.Add(etiquetaError);
      return contenido;
    }

    private Control ConstruirResultado()
    {
      var resultado = new Panel
      {
        BackColor = Estilos.FondoResultado,
        Dock = DockStyle.Fill,
        Height = 96,
        Padding = new Padding(26, 14, 26, 14),
        Margin = new Padding(0),
      };
      // Franja azul a la izquierda, como en la web.
      resultado.Controls.Add(etiquetaResultado);
      resultado.Controls.Add(new Label
      {
        Text = "Resultado",
        Font = Estilos.FuentePequena,
        ForeColor = Estilos.Primario,
        AutoSize = true,
        Dock = DockStyle.Top,
        Padding = new Padding(0, 0, 0, 8),
      });
      etiquetaResultado.Dock = DockStyle.Fill;
      resultado.Paint += (_, e) =>
      {
        using var pincel = new SolidBrush(Estilos.Primario);
        e.Graphics.FillRectangle(pincel, 0, 0, 6, resultado.Height);
      };
      return resultado;
    }

    private static Control Campo(string texto, Control control)
    {
      var campo = new TableLayoutPanel { ColumnCount = 1, Dock = DockStyle.Fill, AutoSize = true, Margin = new Padding(0, 0, 12, 0) };
      campo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
      control.Margin = new Padding(0);
      campo.Controls.Add(Estilos.Etiqueta(texto));
      campo.Controls.Add(control);
      return campo;
    }

    private void CambiarMagnitud()
    {
      if (actualizando)
        return;
      EnSegundoPlano(accionCambiarMagnitud);
    }

    private void Convertir()
    {
      if (!botonConvertir.Enabled)
        return;
      EnSegundoPlano(accionConvertir);
    }

    // Todos los controles quedan bloqueados mientras corre la acción.
    private void EnSegundoPlano(Func<Task> accion)
    {
      Estilos.Ejecutar(accion, MostrarError, comboCategoria, comboDesde, comboHacia, campoValor, botonConvertir);
    }

    private static string? Seleccion(ComboBox combo) => (combo.SelectedItem as Elemento)?.Valor;

    public void MostrarMagnitudes(IReadOnlyList<string> magnitudes)
    {
      actualizando = true;
      comboCategoria.Items.Clear();
      foreach (var m in magnitudes)
        comboCategoria.Items.Add(new Elemento(m, Magnitudes.NombreVisible(m)));
      if (comboCategoria.Items.Count > 0)
        comboCategoria.SelectedIndex = 0;
      actualizando = false;
    }

    public void MostrarUnidades(IReadOnlyList<string> unidades)
    {
      actualizando = true;
      comboDesde.Items.Clear();
      comboHacia.Items.Clear();
      foreach (var u in unidades)
      {
        var elemento = new Elemento(u, Estilos.UnidadVisible(u, NombresUnidades.Nombre(u)));
        comboDesde.Items.Add(elemento);
        comboHacia.Items.Add(elemento);
      }
      if (unidades.Count > 0)
      {
        comboDesde.SelectedIndex = 0;
        comboHacia.SelectedIndex = unidades.Count > 1 ? 1 : 0;
      }
      actualizando = false;
    }

    public string? Magnitud => Seleccion(comboCategoria);

    public string Valor => campoValor.Text;

    public string? UnidadOrigen => Seleccion(comboDesde);

    public string? UnidadDestino => Seleccion(comboHacia);

    public void MostrarResultado(string texto)
    {
      etiquetaResultado.Text = texto;
    }

    public void MostrarError(string mensaje)
    {
      etiquetaError.Text = mensaje;
    }

    public void LimpiarMensajes()
    {
      etiquetaResultado.Text = "";
      etiquetaError.Text = "";
    }

    public void AlConvertir(Func<Task> accion)
    {
      accionConvertir = accion;
    }

    public void AlCambiarMagnitud(Func<Task> accion)
    {
      accionCambiarMagnitud = accion;
    }

    // Permite a las pruebas simular lo que haría el usuario.
    internal ComboBox ComboCategoria => comboCategoria;
    internal ComboBox ComboDesde => comboDesde;
    internal ComboBox ComboHacia => comboHacia;
    internal TextBox CampoValor => campoValor;
    internal Label EtiquetaResultado => etiquetaResultado;
    internal Label EtiquetaError => etiquetaError;
  }
}
