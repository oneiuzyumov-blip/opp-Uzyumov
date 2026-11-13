using System.Windows;
using System.Windows.Media;

namespace PaintLab;

public class LineShape : Shape
{
    public LineShape(Point start, Color color, int width) : base(start, color, width) { }

    public override void Draw(DrawingContext dc) => dc.DrawLine(CreatePen(), Start, End);

    public override bool Contains(Point p) => DistanceToSegment(p, Start, End) <= Tolerance;
}
