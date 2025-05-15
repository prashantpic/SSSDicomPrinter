namespace TheSSS.DICOMViewer.Domain.Validation.Measurements;
using FluentValidation;
using TheSSS.DICOMViewer.Domain.ValueObjects.Measurements;

public class MeasurementValueValidator : AbstractValidator<MeasurementValue>
{
    public MeasurementValueValidator()
    {
        RuleFor(x => x.Unit)
            .IsInEnum()
            .WithMessage("Invalid measurement unit");

        When(x => x.Unit == MeasurementUnit.Millimeters || x.Unit == MeasurementUnit.SquareMillimeters, () => 
        {
            RuleFor(x => x.Value)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Measurement value must be non-negative");
        });
    }
}