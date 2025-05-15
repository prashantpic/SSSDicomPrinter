using FluentValidation;
using DICOMViewer.Domain.ValueObjects.Measurements;

namespace DICOMViewer.Domain.Validation.Measurements
{
    public class MeasurementValueValidator : AbstractValidator<MeasurementValue>
    {
        public MeasurementValueValidator()
        {
            RuleFor(v => v.Value)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Measurement value must be non-negative");
            
            RuleFor(v => v.Unit)
                .IsInEnum();
        }
    }
}