namespace TheSSS.DICOMViewer.Domain.ValueObjects.Measurements;
public readonly record struct Length
{
    public MeasurementValue Value { get; }

    private Length(MeasurementValue value)
    {
        if (value.Unit != MeasurementUnit.Millimeters)
            throw new ArgumentException("Invalid unit for length measurement");
        
        Value = value;
    }

    public static Length Create(double value)
    {
        var measurementValue = MeasurementValue.Create(value, MeasurementUnit.Millimeters);
        return new Length(measurementValue);
    }
}