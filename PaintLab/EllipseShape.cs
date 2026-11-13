using System.Windows;
using System.Windows.Media;

namespace PaintLab;

public class EllipseShape : Shape
{
    public EllipseShape(Point start, Color color, int width) : base(start, color, width) { }

    public override void Draw(DrawingContext dc)
    {
        var r = Bounds();
        var center = new Point(r.X + r.Width / 2, r.Y + r.Height / 2);
        dc.DrawEllipse(null, CreatePen(), center, r.Width / 2, r.Height / 2);
    }

    public override bool Contains(Point p)
    {
        var r = Bounds();
        if (r.Width == 0 || r.Height == 0) return false;
        double dx = (p.X - (r.X + r.Width / 2)) / (r.Width / 2);
        double dy = (p.Y - (r.Y + r.Height / 2)) / (r.Height / 2);
        return dx * dx + dy * dy <= 1;
    }
}
