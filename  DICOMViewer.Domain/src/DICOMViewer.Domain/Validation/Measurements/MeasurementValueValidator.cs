using FluentValidation;
using TheSSS.DICOMViewer.Domain.ValueObjects.Measurements;

namespace TheSSS.DICOMViewer.Domain.Validation.Measurements;

public class MeasurementValueValidator : AbstractValidator<MeasurementValue>
{
    public MeasurementValueValidator()
    {
        RuleFor(mv => mv.Unit)
            .IsInEnum().WithMessage("Invalid measurement unit");

        When(mv => mv.Unit == MeasurementUnit.Millimeters || mv.Unit == MeasurementUnit.SquareMillimeters, () => 
        {
            RuleFor(mv => mv.Value)
                .GreaterThanOrEqualTo(0).WithMessage("Measurement value cannot be negative");
        });
    }
}