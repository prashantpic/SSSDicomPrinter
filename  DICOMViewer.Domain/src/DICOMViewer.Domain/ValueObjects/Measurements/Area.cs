namespace TheSSS.DICOMViewer.Domain.ValueObjects.Measurements;
public readonly record struct Area
{
    public MeasurementValue Value { get; }

    private Area(MeasurementValue value)
    {
        if (value.Unit != MeasurementUnit.SquareMillimeters)
            throw new ArgumentException("Invalid unit for area measurement");
        
        Value = value;
    }

    public static Area Create(double value)
    {
        var measurementValue = MeasurementValue.Create(value, MeasurementUnit.SquareMillimeters);
        return new Area(measurementValue);
    }
}