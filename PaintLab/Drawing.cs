using System.IO;
using System.Text.Json;
using System.Windows;
using System.Windows.Media;

namespace PaintLab;

public class Drawing
{
    private readonly List<Shape> _shapes = new();

    public IReadOnlyList<Shape> Shapes => _shapes;

    public void Add(Shape shape) => _shapes.Add(shape);

    public void Remove(Shape shape) => _shapes.Remove(shape);

    public int IndexOf(Shape shape) => _shapes.IndexOf(shape);

    public void Insert(int index, Shape shape) => _shapes.Insert(index, shape);

    public void UndoLast()
    {
        if (_shapes.Count > 0) _shapes.RemoveAt(_shapes.Count - 1);
    }

    public void Clear() => _shapes.Clear();

    public Shape? HitTest(Point p) => _shapes.LastOrDefault(s => s.Contains(p));

    public void Draw(DrawingContext dc)
    {
        foreach (var shape in _shapes)
            shape.Draw(dc);
    }

    public void Save(string path)
    {
        var data = _shapes.Select(s => new ShapeDto(
            s.GetType().Name,
            s.Color.ToString(),
            s.Width,
            s.GetPoints().Select(p => new PointDto(p.X, p.Y)).ToList())).ToList();

        File.WriteAllText(path, JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));
    }

    public void Load(string path)
    {
        var data = JsonSerializer.Deserialize<List<ShapeDto>>(File.ReadAllText(path)) ?? new();
        var loaded = new List<Shape>();

        foreach (var d in data)
        {
            var color = (Color)ColorConverter.ConvertFromString(d.Color)!;
            var first = d.Points[0];

            var shape = ShapeFactory.Create(d.Kind, new Point(first.X, first.Y), color, d.Width);

            foreach (var p in d.Points.Skip(1))
                shape.MoveEnd(new Point(p.X, p.Y));

            loaded.Add(shape);
        }

        _shapes.Clear();
        _shapes.AddRange(loaded);
    }
}
