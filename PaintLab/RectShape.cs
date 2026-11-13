using System.Windows;
using System.Windows.Media;

namespace PaintLab;

public class RectShape : Shape
{
    public RectShape(Point start, Color color, int width) : base(start, color, width) { }

    public override void Draw(DrawingContext dc) => dc.DrawRectangle(null, CreatePen(), Bounds());

    public override bool Contains(Point p) => Bounds().Contains(p);
}
