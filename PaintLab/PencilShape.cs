using System.Windows;
using System.Windows.Media;

namespace PaintLab;

public class PencilShape : Shape
{
    private readonly List<Point> _points = new();

    public PencilShape(Point start, Color color, int width) : base(start, color, width)
    {
        _points.Add(start);
    }

    public override void MoveEnd(Point p)
    {
        base.MoveEnd(p);
        _points.Add(p);
    }

    public override IReadOnlyList<Point> GetPoints() => _points;

    public override void Draw(DrawingContext dc)
    {
        if (_points.Count < 2) return;

        var geometry = new StreamGeometry();
        using (var ctx = geometry.Open())
        {
            ctx.BeginFigure(_points[0], false, false);
            ctx.PolyLineTo(_points.Skip(1).ToList(), true, true);
        }
        geometry.Freeze();
        dc.DrawGeometry(null, CreatePen(), geometry);
    }

    public override bool Contains(Point p)
    {
        for (int i = 1; i < _points.Count; i++)
            if (DistanceToSegment(p, _points[i - 1], _points[i]) <= Tolerance)
                return true;
        return false;
    }

    public override void Move(Vector d)
    {
        base.Move(d);
        for (int i = 0; i < _points.Count; i++)
            _points[i] += d;
    }

    public override Rect Bounds()
    {
        var r = new Rect(_points[0], _points[0]);
        foreach (var p in _points) r.Union(p);
        return r;
    }
}
