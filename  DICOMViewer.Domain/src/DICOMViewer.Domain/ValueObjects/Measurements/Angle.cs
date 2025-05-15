using DICOMViewer.Domain.Validation.Measurements;
using DICOMViewer.Domain.ValueObjects.Measurements;

namespace DICOMViewer.Domain.ValueObjects.Measurements
{
    public readonly record struct Angle
    {
        public MeasurementValue Value { get; }

        private Angle(MeasurementValue value)
        {
            if (value.Unit != MeasurementUnit.Degrees)
                throw new ValidationException("Angle must be in degrees");
            
            Value = value;
        }

        public static Angle Create(double value)
        {
            var measurement = MeasurementValue.Create(value, MeasurementUnit.Degrees);
            return new Angle(measurement);
        }
    }
}