using System.Diagnostics;

namespace ec.edu.monster.vista
{
  /// <summary>
  /// Paleta, fuentes y pequeñas fábricas de controles compartidas por las ventanas.
  /// Mismos colores que el cliente de escritorio de Java.
  /// </summary>
  internal static class Estilos
  {
    public const string TituloVentana = "Conversión de Unidades Monster";
    public const string ErrorInesperado = "Ocurrió un error inesperado";

    public static readonly Color Primario = Color.FromArgb(0x1F, 0x4E, 0x79);
    public static readonly Color Fondo = Color.FromArgb(0xF4, 0xF6, 0xF8);
    public static readonly Color Texto = Color.FromArgb(0x1F, 0x29, 0x37);
    public static readonly Color Borde = Color.FromArgb(0xD1, 0xD5, 0xDB);
    public static readonly Color FondoResultado = Color.FromArgb(0xE8, 0xF0, 0xF8);
    public static readonly Color Exito = Color.FromArgb(0x15, 0x80, 0x3D);
    public static readonly Color Error = Color.FromArgb(0xB9, 0x1C, 0x1C);
    public static readonly Color Blanco = Color.White;

    public static readonly Font Fuente = new("Segoe UI", 11f);
    public static readonly Font FuenteNegrita = new("Segoe UI", 11f, FontStyle.Bold);
    public static readonly Font FuenteEtiqueta = new("Segoe UI", 10f, FontStyle.Bold);
    public static readonly Font FuentePequena = new("Segoe UI", 9.5f, FontStyle.Bold);
    public static readonly Font FuenteTitulo = new("Segoe UI", 19f, FontStyle.Bold);
    public static readonly Font FuenteResultado = new("Segoe UI", 17f, FontStyle.Bold);

    public static Label Titulo(string texto) => new()
    {
      Text = texto,
      Font = FuenteTitulo,
      ForeColor = Primario,
      AutoSize = true,
      Margin = new Padding(0, 0, 0, 16),
    };

    public static Label Etiqueta(string texto) => new()
    {
      Text = texto,
      Font = FuenteEtiqueta,
      ForeColor = Texto,
      AutoSize = true,
      Margin = new Padding(0, 0, 0, 4),
    };

    public static Label MensajeError() => new()
    {
      Text = "",
      Font = Fuente,
      ForeColor = Error,
      AutoSize = true,
      Margin = new Padding(0, 8, 0, 0),
    };

    public static TextBox Entrada(bool clave = false) => new()
    {
      Font = Fuente,
      ForeColor = Texto,
      BackColor = Blanco,
      BorderStyle = BorderStyle.FixedSingle,
      UseSystemPasswordChar = clave,
      Dock = DockStyle.Fill,
      Margin = new Padding(0, 0, 0, 14),
    };

    public static ComboBox Combo() => new()
    {
      Font = Fuente,
      ForeColor = Texto,
      BackColor = Blanco,
      DropDownStyle = ComboBoxStyle.DropDownList,
      FlatStyle = FlatStyle.Flat,
      Dock = DockStyle.Fill,
    };

    public static Button BotonPrimario(string texto)
    {
      var boton = new Button
      {
        Text = texto,
        Font = FuenteNegrita,
        ForeColor = Blanco,
        BackColor = Primario,
        FlatStyle = FlatStyle.Flat,
        Cursor = Cursors.Hand,
        AutoSize = true,
        Padding = new Padding(16, 4, 16, 4),
        UseVisualStyleBackColor = false,
      };
      boton.FlatAppearance.BorderSize = 0;
      return boton;
    }

    /// <summary>"Metro (m)", o solo el símbolo si no se conoce su nombre.</summary>
    public static string UnidadVisible(string simbolo, string nombre)
    {
      return nombre == simbolo ? simbolo : nombre + " (" + simbolo + ")";
    }

    public static MenuStrip BarraMenu(params ToolStripMenuItem[] menus)
    {
      var barra = new MenuStrip { BackColor = Blanco, Font = Fuente };
      barra.Items.AddRange(menus);
      return barra;
    }

    public static ToolStripMenuItem Menu(string texto, params ToolStripItem[] opciones)
    {
      var menu = new ToolStripMenuItem(texto) { ForeColor = Texto };
      menu.DropDownItems.AddRange(opciones);
      return menu;
    }

    public static ToolStripMenuItem Opcion(string texto, Action accion)
    {
      return new ToolStripMenuItem(texto, null, (_, _) => accion());
    }

    public static ToolStripMenuItem MenuAyuda(IWin32Window padre)
    {
      return Menu("Ayuda", Opcion("Acerca de", () =>
        MessageBox.Show(padre, TituloVentana, "Acerca de", MessageBoxButtons.OK, MessageBoxIcon.Information)));
    }

    /// <summary>
    /// Ejecuta una acción del controlador sin congelar la ventana. Los controles quedan
    /// deshabilitados mientras dura, así no se lanza una segunda acción encima.
    /// </summary>
    public static async void Ejecutar(Func<Task> accion, Action<string> alFallar, params Control[] controles)
    {
      foreach (var control in controles)
        control.Enabled = false;

      try
      {
        await accion();
      }
      catch (Exception e)
      {
        Debug.WriteLine(e);
        alFallar(ErrorInesperado);
      }
      finally
      {
        foreach (var control in controles)
        {
          if (!control.IsDisposed)
            control.Enabled = true;
        }
      }
    }
  }

  /// <summary>Elemento de una lista: guarda el valor del servidor y muestra un texto.</summary>
  internal sealed record Elemento(string Valor, string Texto)
  {
    public override string ToString() => Texto;
  }
}
