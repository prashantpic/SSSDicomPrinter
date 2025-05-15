namespace TheSSS.DICOMViewer.Domain.ValueObjects;

public record struct ImageCoordinates(double X, double Y)
{
    public override string ToString() => $"({X:F1}, {Y:F1})";
}