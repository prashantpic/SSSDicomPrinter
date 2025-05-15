namespace TheSSS.DICOMViewer.Domain.ValueObjects;
public readonly record struct ImageCoordinates
{
    public double X { get; }
    public double Y { get; }

    public ImageCoordinates(double x, double y)
    {
        X = x;
        Y = y;
    }
}