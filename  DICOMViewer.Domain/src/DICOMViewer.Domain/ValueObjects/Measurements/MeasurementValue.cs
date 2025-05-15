namespace TheSSS.DICOMViewer.Domain.ValueObjects.Measurements;

public record struct MeasurementValue
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
        if (unit is MeasurementUnit.Millimeters or MeasurementUnit.SquareMillimeters && value < 0)
            throw new MeasurementCalculationException("Measurement value cannot be negative");

        return new MeasurementValue(value, unit);
    }
}