namespace PaintLab;

public record PointDto(double X, double Y);
public record ShapeDto(string Kind, string Color, int Width, List<PointDto> Points);
