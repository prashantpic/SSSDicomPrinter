namespace TheSSS.DICOMViewer.Domain.DomainServices;
using TheSSS.DICOMViewer.Domain.ValueObjects;
using TheSSS.DICOMViewer.Domain.ValueObjects.Measurements;
using TheSSS.DICOMViewer.Domain.Exceptions;

public class MeasurementCalculator
{
    public Length CalculateLength(ImageCoordinates start, ImageCoordinates end, PixelSpacing spacing)
    {
        if (spacing.RowSpacing <= 0 || spacing.ColumnSpacing <= 0)
            throw new MeasurementCalculationException("Invalid pixel spacing values");

        double dx = (end.X - start.X) * spacing.ColumnSpacing;
        double dy = (end.Y - start.Y) * spacing.RowSpacing;
        double length = Math.Sqrt(dx * dx + dy * dy);
        
        return Length.Create(length);
    }

    public Angle CalculateAngle(ImageCoordinates p1, ImageCoordinates p2, ImageCoordinates p3, PixelSpacing spacing)
    {
        if (spacing.RowSpacing <= 0 || spacing.ColumnSpacing <= 0)
            throw new MeasurementCalculationException("Invalid pixel spacing values");

        var a = CalculateDistance(p2, p1, spacing);
        var b = CalculateDistance(p3, p2, spacing);
        var c = CalculateDistance(p3, p1, spacing);

        double angleRad = Math.Acos((a.Value * a.Value + b.Value * b.Value - c.Value * c.Value) 
                         / (2 * a.Value * b.Value));
        double angleDeg = angleRad * (180 / Math.PI);
        
        return Angle.Create(angleDeg);
    }

    public Area CalculateArea(IEnumerable<ImageCoordinates> vertices, PixelSpacing spacing)
    {
        var points = vertices.ToList();
        if (points.Count < 3)
            throw new MeasurementCalculationException("At least 3 vertices required for area calculation");

        double area = 0;
        for (int i = 0; i < points.Count; i++)
        {
            var current = points[i];
            var next = points[(i + 1) % points.Count];
            area += (current.X * next.Y - next.X * current.Y);
        }
        area = Math.Abs(area * 0.5 * spacing.RowSpacing * spacing.ColumnSpacing);
        
        return Area.Create(area);
    }

    private static Length CalculateDistance(ImageCoordinates a, ImageCoordinates b, PixelSpacing spacing)
    {
        double dx = (b.X - a.X) * spacing.ColumnSpacing;
        double dy = (b.Y - a.Y) * spacing.RowSpacing;
        return Length.Create(Math.Sqrt(dx * dx + dy * dy));
    }
}