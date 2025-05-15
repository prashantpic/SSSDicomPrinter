namespace TheSSS.DICOMViewer.Domain.ValueObjects.Measurements;

public record struct Angle
{
    public double Value { get; }
    public MeasurementUnit Unit { get; }

    private Angle(double value, MeasurementUnit unit)
    {
        Value = value;
        Unit = unit;
    }

    public static Angle FromDegrees(double value)
    {
        return new Angle(value, MeasurementUnit.Degrees);
    }

    public override string ToString() => $"{Value:F1}°";
}