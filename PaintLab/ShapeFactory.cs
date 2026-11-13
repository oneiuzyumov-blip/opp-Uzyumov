using System.Windows;
using System.Windows.Media;

namespace PaintLab;

public static class ShapeFactory
{
    public static readonly Dictionary<string, (string Title, Func<Point, Color, int, Shape> Create)> Kinds = new()
    {
        ["LineShape"]    = ("Линия",         (p, c, w) => new LineShape(p, c, w)),
        ["RectShape"]    = ("Прямоугольник", (p, c, w) => new RectShape(p, c, w)),
        ["EllipseShape"] = ("Эллипс",        (p, c, w) => new EllipseShape(p, c, w)),
        ["PencilShape"]  = ("Карандаш",      (p, c, w) => new PencilShape(p, c, w)),
    };

    public static Shape Create(string kind, Point start, Color color, int width) =>
        Kinds[kind].Create(start, color, width);
}
