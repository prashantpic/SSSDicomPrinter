namespace TheSSS.DICOMViewer.Domain.ValueObjects.Measurements;

public record struct Area
{
    public double Value { get; }
    public MeasurementUnit Unit { get; }

    private Area(double value, MeasurementUnit unit)
    {
        Value = value;
        Unit = unit;
    }

    public static Area FromSquareMillimeters(double value)
    {
        if (value < 0)
            throw new MeasurementCalculationException("Area cannot be negative");

        return new Area(value, MeasurementUnit.SquareMillimeters);
    }

    public override string ToString() => $"{Value:F2} mm²";
}