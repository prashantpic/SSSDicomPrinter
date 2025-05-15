namespace TheSSS.DICOMViewer.Domain.ValueObjects.Measurements;

public record struct Length
{
    public double Value { get; }
    public MeasurementUnit Unit { get; }

    private Length(double value, MeasurementUnit unit)
    {
        Value = value;
        Unit = unit;
    }

    public static Length FromMillimeters(double value)
    {
        if (value < 0)
            throw new MeasurementCalculationException("Length cannot be negative");

        return new Length(value, MeasurementUnit.Millimeters);
    }

    public override string ToString() => $"{Value:F2} {Unit}";
}