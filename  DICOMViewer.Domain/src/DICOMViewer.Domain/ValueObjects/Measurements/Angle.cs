namespace TheSSS.DICOMViewer.Domain.ValueObjects.Measurements;
public readonly record struct Angle
{
    public MeasurementValue Value { get; }

    private Angle(MeasurementValue value)
    {
        if (value.Unit != MeasurementUnit.Degrees)
            throw new ArgumentException("Invalid unit for angle measurement");
        
        Value = value;
    }

    public static Angle Create(double value)
    {
        var measurementValue = MeasurementValue.Create(value, MeasurementUnit.Degrees);
        return new Angle(measurementValue);
    }
}