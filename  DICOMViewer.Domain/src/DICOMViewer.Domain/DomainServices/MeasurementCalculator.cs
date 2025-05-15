using TheSSS.DICOMViewer.Domain.ValueObjects;
using TheSSS.DICOMViewer.Domain.ValueObjects.Measurements;

namespace TheSSS.DICOMViewer.Domain.DomainServices;

public class MeasurementCalculator
{
    public Length CalculateLength(ImageCoordinates start, ImageCoordinates end, PixelSpacing spacing)
    {
        ValidateSpacing(spacing);
        
        var dx = (end.X - start.X) * spacing.ColumnSpacing;
        var dy = (end.Y - start.Y) * spacing.RowSpacing;
        var distance = Math.Sqrt(dx * dx + dy * dy);
        
        return Length.FromMillimeters(distance);
    }

    public Angle CalculateAngle(ImageCoordinates p1, ImageCoordinates p2, ImageCoordinates p3, PixelSpacing spacing)
    {
        ValidateSpacing(spacing);
        
        var v1 = new Vector(p1, p2);
        var v2 = new Vector(p3, p2);
        var angleRadians = Math.Acos(v1.DotProduct(v2) / (v1.Magnitude * v2.Magnitude));
        
        return Angle.FromRadians(angleRadians);
    }

    public Area CalculateArea(IEnumerable<ImageCoordinates> vertices, PixelSpacing spacing)
    {
        ValidateSpacing(spacing);
        
        var points = vertices.ToArray();
        if (points.Length < 3)
            throw new MeasurementCalculationException("At least 3 vertices required for area calculation");

        double area = 0;
        for (int i = 0; i < points.Length; i++)
        {
            var j = (i + 1) % points.Length;
            area += points[i].X * points[j].Y - points[j].X * points[i].Y;
        }
        
        var pixelArea = Math.Abs(area / 2.0);
        var physicalArea = pixelArea * spacing.RowSpacing * spacing.ColumnSpacing;
        
        return Area.FromSquareMillimeters(physicalArea);
    }

    private void ValidateSpacing(PixelSpacing spacing)
    {
        if (spacing.RowSpacing <= 0 || spacing.ColumnSpacing <= 0)
            throw new MeasurementCalculationException("Invalid pixel spacing values");
    }

    private readonly struct Vector
    {
        public readonly double X;
        public readonly double Y;

        public Vector(ImageCoordinates start, ImageCoordinates end)
        {
            X = end.X - start.X;
            Y = end.Y - start.Y;
        }

        public double DotProduct(Vector other) => X * other.X + Y * other.Y;
        public double Magnitude => Math.Sqrt(X * X + Y * Y);
    }
}