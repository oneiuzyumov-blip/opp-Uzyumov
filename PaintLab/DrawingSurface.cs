using System.Windows;
using System.Windows.Media;

namespace PaintLab;

public class DrawingSurface : FrameworkElement
{
    public Drawing? Drawing { get; set; }
    public Shape? Preview { get; set; }
    public Shape? Selected { get; set; }

    public DrawingSurface() => ClipToBounds = true;

    protected override void OnRender(DrawingContext dc)
    {
        dc.DrawRectangle(Brushes.White, null, new Rect(0, 0, ActualWidth, ActualHeight));
        Drawing?.Draw(dc);
        Preview?.Draw(dc);

        if (Selected != null)
        {
            var r = Selected.Bounds();
            r.Inflate(6, 6);
            dc.DrawRectangle(null, new Pen(Brushes.DodgerBlue, 1) { DashStyle = DashStyles.Dash }, r);
        }
    }
}
