using DICOMViewer.Domain.Validation.Measurements;
using DICOMViewer.Domain.ValueObjects.Measurements;

namespace DICOMViewer.Domain.ValueObjects.Measurements
{
    public readonly record struct Area
    {
        public MeasurementValue Value { get; }

        private Area(MeasurementValue value)
        {
            if (value.Unit != MeasurementUnit.SquareMillimeters)
                throw new ValidationException("Area must be in square millimeters");
            
            Value = value;
        }

        public static Area Create(double value)
        {
            var measurement = MeasurementValue.Create(value, MeasurementUnit.SquareMillimeters);
            return new Area(measurement);
        }
    }
}