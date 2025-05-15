using FluentValidation;
using TheSSS.DICOMViewer.Domain.ValueObjects.Measurements;

namespace TheSSS.DICOMViewer.Domain.Validation.Measurements;

public class MeasurementValueValidator : AbstractValidator<MeasurementValue>
{
    public MeasurementValueValidator()
    {
        RuleFor(x => x.Value)
            .GreaterThanOrEqualTo(0).When(x => x.Unit != MeasurementUnit.Degrees)
            .WithMessage("Measurement value must be non-negative");
        
        RuleFor(x => x.Unit)
            .IsInEnum().WithMessage("Invalid measurement unit");
    }
}