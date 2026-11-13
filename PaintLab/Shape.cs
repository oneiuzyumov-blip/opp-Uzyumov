using System.Windows;
using System.Windows.Media;

namespace PaintLab;

public abstract class Shape
{
    public Point Start { get; protected set; }
    public Point End { get; protected set; }
    public Color Color { get; }
    public int Width { get; }

    protected Shape(Point start, Color color, int width)
    {
        Start = End = start;
        Color = color;
        Width = width;
    }

    public virtual void MoveEnd(Point p) => End = p;

    public virtual Rect Bounds() => new(
        Math.Min(Start.X, End.X), Math.Min(Start.Y, End.Y),
        Math.Abs(End.X - Start.X), Math.Abs(End.Y - Start.Y));

    public virtual IReadOnlyList<Point> GetPoints() => new[] { Start, End };

    public virtual void Move(Vector d)
    {
        Start += d;
        End += d;
    }

    protected double Tolerance => Width / 2.0 + 4;

    protected static double DistanceToSegment(Point p, Point a, Point b)
    {
        var ab = b - a;
        double len2 = ab.LengthSquared;
        if (len2 == 0) return (p - a).Length;
        double t = Math.Clamp((p - a) * ab / len2, 0, 1);
        return (p - (a + ab * t)).Length;
    }

    protected Pen CreatePen() => new(new SolidColorBrush(Color), Width)
    {
        StartLineCap = PenLineCap.Round,
        EndLineCap = PenLineCap.Round,
        LineJoin = PenLineJoin.Round
    };

    public abstract void Draw(DrawingContext dc);

    public abstract bool Contains(Point p);
}
