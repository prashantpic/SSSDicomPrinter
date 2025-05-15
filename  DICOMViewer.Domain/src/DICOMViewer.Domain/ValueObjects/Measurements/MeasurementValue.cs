using DICOMViewer.Domain.Validation.Measurements;

namespace DICOMViewer.Domain.ValueObjects.Measurements
{
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
            new MeasurementValueValidator().ValidateAndThrow(measurement);
            return measurement;
        }
    }
}