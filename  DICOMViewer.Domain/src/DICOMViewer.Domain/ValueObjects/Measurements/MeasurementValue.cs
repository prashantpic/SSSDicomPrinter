namespace TheSSS.DICOMViewer.Domain.ValueObjects.Measurements;
public readonly record struct MeasurementValue
{
    public double Value { get; }
    public MeasurementUnit Unit { get; }

    private MeasurementValue(double value, MeasurementUnit unit)
    {
        Value = value;
        Unit = unit;
    }

    public static MeasurementValue Create(double value, MeasurementUnit unit)
    {
        var measurement = new MeasurementValue(value, unit);
        var validator = new MeasurementValueValidator();
        var result = validator.Validate(measurement);
        
        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        return measurement;
    }
}