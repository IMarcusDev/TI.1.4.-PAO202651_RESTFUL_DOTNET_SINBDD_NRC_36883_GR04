using System.Drawing.Drawing2D;

namespace ec.edu.monster.vista
{
  /// <summary>
  /// Punto verde o rojo con un texto, que muestra si el servidor responde.
  /// Al abrirse la ventana pide revisar el estado y luego lo hace cada 10 segundos.
  /// </summary>
  public class IndicadorEstado : Control, IEstadoVista
  {
    public const int IntervaloMs = 10_000;

    private readonly string textoDisponible;
    private readonly string textoCaido;
    private readonly System.Windows.Forms.Timer temporizador = new() { Interval = IntervaloMs };
    private Func<Task> accionRevisar = () => Task.CompletedTask;
    private bool revisando;

    public IndicadorEstado(string textoDisponible, string textoCaido)
    {
      this.textoDisponible = textoDisponible;
      this.textoCaido = textoCaido;
      SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer
        | ControlStyles.UserPaint | ControlStyles.SupportsTransparentBackColor, true);
      BackColor = Color.Transparent;
      Font = Estilos.FuentePequena;
      Height = 28;
      Dock = DockStyle.Bottom;
      Padding = new Padding(34, 0, 34, 8);
      temporizador.Tick += (_, _) => Revisar();

      // Hasta la primera respuesta no se sabe nada: ni verde ni rojo.
      Text = TextoComprobando;
      ForeColor = Color.Gray;
    }

    public const string TextoComprobando = "Comprobando servidor…";

    /// <summary>True si el último resultado fue "servidor disponible".</summary>
    public bool Disponible { get; private set; }

    public void MostrarEstado(bool disponible)
    {
      Disponible = disponible;
      Text = disponible ? textoDisponible : textoCaido;
      ForeColor = disponible ? Estilos.Exito : Estilos.Error;
      Invalidate();
    }

    public void AlRevisarEstado(Func<Task> accion)
    {
      accionRevisar = accion;
    }

    /// <summary>Revisa ya y luego cada 10 segundos. La ventana lo llama al mostrarse.</summary>
    public void Iniciar()
    {
      Revisar();
      temporizador.Start();
    }

    // Una revisión lenta no debe solaparse con la siguiente.
    private async void Revisar()
    {
      if (revisando || IsDisposed)
        return;

      revisando = true;
      try
      {
        await accionRevisar();
      }
      catch (Exception)
      {
        MostrarEstado(false);
      }
      finally
      {
        revisando = false;
      }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
      int diametro = (int)(Font.Height * 0.6);
      int y = (Height - Padding.Bottom - diametro) / 2;

      e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
      using (var pincel = new SolidBrush(ForeColor))
        e.Graphics.FillEllipse(pincel, Padding.Left, y, diametro, diametro);

      var areaTexto = new Rectangle(Padding.Left + diametro + 8, 0,
        Width - Padding.Horizontal - diametro - 8, Height - Padding.Bottom);
      TextRenderer.DrawText(e.Graphics, Text, Font, areaTexto, ForeColor,
        TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing)
        temporizador.Dispose();
      base.Dispose(disposing);
    }
  }
}
