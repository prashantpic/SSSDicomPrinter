using DICOMViewer.Domain.Validation.Measurements;
using DICOMViewer.Domain.ValueObjects.Measurements;

namespace DICOMViewer.Domain.ValueObjects.Measurements
{
    public readonly record struct Length
    {
        public MeasurementValue Value { get; }

        private Length(MeasurementValue value)
        {
            if (value.Unit != MeasurementUnit.Millimeters)
                throw new ValidationException("Length must be in millimeters");
            
            Value = value;
        }

        public static Length Create(double value)
        {
            var measurement = MeasurementValue.Create(value, MeasurementUnit.Millimeters);
            return new Length(measurement);
        }
    }
}