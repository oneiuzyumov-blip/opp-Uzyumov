using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;

namespace PaintLab;

public record ColorItem(string Name, Color Color);

public partial class MainWindow : Window
{
    private const string SelectTool = "Select";

    private readonly Drawing _drawing = new();
    private readonly History _history = new();
    private string _tool = "LineShape";
    private Shape? _current;
    private Shape? _selected;
    private bool _dragging;
    private Point _last;

    public MainWindow()
    {
        InitializeComponent();
        Surface.Drawing = _drawing;
        BuildToolButtons();
        BuildColors();
        Status.Text = $"Инструмент: {ShapeFactory.Kinds[_tool].Title}";
    }

    private void AddToolButton(string kind, string title)
    {
        var rb = new RadioButton
        {
            Content = title,
            GroupName = "tools",
            Style = (Style)FindResource("ToolButton"),
            IsChecked = kind == _tool
        };
        rb.Checked += (_, _) =>
        {
            _tool = kind;
            Status.Text = $"Инструмент: {title}";
            if (kind != SelectTool)
            {
                ClearSelection();
                Redraw();
            }
        };
        ToolPanel.Children.Add(rb);
    }

    private void BuildToolButtons()
    {
        AddToolButton(SelectTool, "Выделение");
        foreach (var (kind, info) in ShapeFactory.Kinds)
            AddToolButton(kind, info.Title);
    }

    private void BuildColors()
    {
        ColorBox.ItemsSource = new[]
        {
            new ColorItem("Чёрный", Colors.Black),
            new ColorItem("Красный", Colors.Red),
            new ColorItem("Зелёный", Colors.Green),
            new ColorItem("Синий", Colors.Blue),
            new ColorItem("Оранжевый", Colors.Orange),
            new ColorItem("Фиолетовый", Colors.Purple),
        };
        ColorBox.SelectedIndex = 0;
    }

    private void Redraw() => Surface.InvalidateVisual();

    private void ClearSelection()
    {
        _selected = null;
        Surface.Selected = null;
    }

    private void Surface_MouseDown(object sender, MouseButtonEventArgs e)
    {
        var p = e.GetPosition(Surface);

        if (_tool == SelectTool)
        {
            _selected = _drawing.HitTest(p);
            Surface.Selected = _selected;
            _last = p;
            _dragging = _selected != null;
            if (_dragging) Surface.CaptureMouse();
            Redraw();
            return;
        }

        var color = ((ColorItem)ColorBox.SelectedItem).Color;
        _current = ShapeFactory.Create(_tool, p, color, (int)WidthSlider.Value);
        Surface.Preview = _current;
        Surface.CaptureMouse();
    }

    private void Surface_MouseMove(object sender, MouseEventArgs e)
    {
        var p = e.GetPosition(Surface);

        if (_dragging && _selected != null)
        {
            _selected.Move(p - _last);
            _last = p;
            Redraw();
            return;
        }

        if (_current == null) return;
        _current.MoveEnd(p);
        Redraw();
    }

    private void Surface_MouseUp(object sender, MouseButtonEventArgs e)
    {
        if (_dragging)
        {
            _dragging = false;
            Surface.ReleaseMouseCapture();
            return;
        }

        if (_current == null) return;

        if (_current.Start != _current.End)
            _history.Do(new AddShapeCommand(_drawing, _current));

        _current = null;
        Surface.Preview = null;
        Surface.ReleaseMouseCapture();
        Redraw();
    }

    private void Undo_Click(object sender, RoutedEventArgs e)
    {
        ClearSelection();
        _history.Undo();
        Redraw();
    }

    private void Redo_Click(object sender, RoutedEventArgs e)
    {
        ClearSelection();
        _history.Redo();
        Redraw();
    }

    private void Clear_Click(object sender, RoutedEventArgs e)
    {
        ClearSelection();
        _history.Do(new ClearCommand(_drawing));
        Redraw();
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Z)
        {
            Undo_Click(sender, e);
            e.Handled = true;
        }
        else if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.Y)
        {
            Redo_Click(sender, e);
            e.Handled = true;
        }
        else if (e.Key == Key.Delete && _selected != null)
        {
            _history.Do(new RemoveShapeCommand(_drawing, _selected));
            ClearSelection();
            Redraw();
            e.Handled = true;
        }
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new SaveFileDialog { Filter = "Рисунок (*.json)|*.json", DefaultExt = ".json" };
        if (dlg.ShowDialog() == true)
            _drawing.Save(dlg.FileName);
    }

    private void Open_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog { Filter = "Рисунок (*.json)|*.json" };
        if (dlg.ShowDialog() != true) return;

        try
        {
            _drawing.Load(dlg.FileName);
            ClearSelection();
            _history.Clear();
            Redraw();
        }
        catch (Exception ex)
        {
            MessageBox.Show("Не удалось открыть файл: " + ex.Message);
        }
    }

    private void Export_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new SaveFileDialog { Filter = "PNG (*.png)|*.png", DefaultExt = ".png" };
        if (dlg.ShowDialog() != true) return;

        int w = (int)Surface.ActualWidth, h = (int)Surface.ActualHeight;

        var visual = new DrawingVisual();
        using (var dc = visual.RenderOpen())
        {
            dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, w, h));
            _drawing.Draw(dc);
        }

        var bmp = new RenderTargetBitmap(w, h, 96, 96, PixelFormats.Pbgra32);
        bmp.Render(visual);

        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bmp));
        using var stream = File.Create(dlg.FileName);
        encoder.Save(stream);
    }
}
