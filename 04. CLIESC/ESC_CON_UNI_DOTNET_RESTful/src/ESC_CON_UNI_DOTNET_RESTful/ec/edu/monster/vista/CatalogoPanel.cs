using ec.edu.monster.modelo;

namespace ec.edu.monster.vista
{
  /// <summary>
  /// Pantalla "Consultar catálogo": magnitudes a la izquierda y unidades de la
  /// elegida a la derecha, con su nombre en español y su símbolo.
  /// </summary>
  public class CatalogoPanel : UserControl, ICatalogoVista
  {
    private readonly ListBox listaMagnitudes = new()
    {
      Font = Estilos.Fuente,
      ForeColor = Estilos.Texto,
      BackColor = Estilos.Blanco,
      BorderStyle = BorderStyle.FixedSingle,
      Dock = DockStyle.Left,
      Width = 210,
      IntegralHeight = false,
      ItemHeight = 40,
      DrawMode = DrawMode.OwnerDrawFixed,
    };
    private readonly ListView tablaUnidades = new()
    {
      Font = Estilos.Fuente,
      ForeColor = Estilos.Texto,
      BackColor = Estilos.Blanco,
      BorderStyle = BorderStyle.FixedSingle,
      Dock = DockStyle.Fill,
      View = View.Details,
      FullRowSelect = true,
      HeaderStyle = ColumnHeaderStyle.Nonclickable,
      MultiSelect = false,
    };
    private readonly Label etiquetaError = Estilos.MensajeError();

    private Func<Task> accionCambiarMagnitud = () => Task.CompletedTask;
    // True mientras la lista se llena desde el código, para no disparar acciones del usuario.
    private bool actualizando;

    public CatalogoPanel()
    {
      BackColor = Estilos.Fondo;
      Font = Estilos.Fuente;
      Dock = DockStyle.Fill;
      Padding = new Padding(34, 24, 34, 8);

      tablaUnidades.Columns.Add("Nombre", 300);
      tablaUnidades.Columns.Add("Símbolo", 140);
      tablaUnidades.Resize += (_, _) => AjustarColumnas();

      var cuerpo = new Panel { Dock = DockStyle.Fill };
      cuerpo.Controls.Add(tablaUnidades);
      cuerpo.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 20 });
      cuerpo.Controls.Add(listaMagnitudes);

      etiquetaError.Dock = DockStyle.Bottom;
      var titulo = Estilos.Titulo("Consultar catálogo");
      titulo.Dock = DockStyle.Top;
      titulo.Padding = new Padding(0, 0, 0, 16);

      Controls.Add(cuerpo);
      Controls.Add(etiquetaError);
      Controls.Add(titulo);

      listaMagnitudes.DrawItem += DibujarMagnitud;
      listaMagnitudes.SelectedIndexChanged += (_, _) => CambiarMagnitud();
    }

    private void AjustarColumnas()
    {
      int ancho = tablaUnidades.ClientSize.Width;
      if (ancho <= 0)
        return;
      tablaUnidades.Columns[0].Width = (int)(ancho * 0.6);
      tablaUnidades.Columns[1].Width = ancho - tablaUnidades.Columns[0].Width;
    }

    // La magnitud elegida se pinta con fondo azul, como en la web.
    private void DibujarMagnitud(object? sender, DrawItemEventArgs e)
    {
      if (e.Index < 0)
        return;

      bool elegida = (e.State & DrawItemState.Selected) != 0;
      using (var fondo = new SolidBrush(elegida ? Estilos.Primario : Estilos.Blanco))
        e.Graphics.FillRectangle(fondo, e.Bounds);

      var texto = listaMagnitudes.Items[e.Index].ToString();
      var area = new Rectangle(e.Bounds.X + 12, e.Bounds.Y, e.Bounds.Width - 12, e.Bounds.Height);
      TextRenderer.DrawText(e.Graphics, texto, elegida ? Estilos.FuenteNegrita : Estilos.Fuente, area,
        elegida ? Estilos.Blanco : Estilos.Texto, TextFormatFlags.VerticalCenter | TextFormatFlags.Left);
    }

    private void CambiarMagnitud()
    {
      if (actualizando)
        return;
      Estilos.Ejecutar(accionCambiarMagnitud, MostrarError, listaMagnitudes);
    }

    public void MostrarMagnitudes(IReadOnlyList<string> magnitudes)
    {
      actualizando = true;
      listaMagnitudes.Items.Clear();
      foreach (var m in magnitudes)
        listaMagnitudes.Items.Add(new Elemento(m, Magnitudes.NombreVisible(m)));
      if (listaMagnitudes.Items.Count > 0)
        listaMagnitudes.SelectedIndex = 0;
      actualizando = false;
    }

    public string? Magnitud => (listaMagnitudes.SelectedItem as Elemento)?.Valor;

    public void MostrarUnidades(IReadOnlyList<string> unidades)
    {
      tablaUnidades.BeginUpdate();
      tablaUnidades.Items.Clear();
      foreach (var u in unidades)
        tablaUnidades.Items.Add(new ListViewItem([NombresUnidades.Nombre(u), u]));
      tablaUnidades.EndUpdate();
    }

    public void MostrarError(string mensaje)
    {
      etiquetaError.Text = mensaje;
    }

    public void LimpiarMensajes()
    {
      etiquetaError.Text = "";
    }

    public void AlCambiarMagnitud(Func<Task> accion)
    {
      accionCambiarMagnitud = accion;
    }

    // Permite a las pruebas simular lo que haría el usuario.
    internal ListBox ListaMagnitudes => listaMagnitudes;
    internal ListView TablaUnidades => tablaUnidades;
    internal Label EtiquetaError => etiquetaError;
  }
}
