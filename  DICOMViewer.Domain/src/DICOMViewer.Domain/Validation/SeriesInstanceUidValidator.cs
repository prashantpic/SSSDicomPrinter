using FluentValidation;
using TheSSS.DICOMViewer.Domain.Aggregates.StudyAggregate.ValueObjects;

namespace TheSSS.DICOMViewer.Domain.Validation;

public class SeriesInstanceUidValidator : AbstractValidator<string>
{
    public SeriesInstanceUidValidator()
    {
        RuleFor(uid => uid)
            .NotEmpty().WithMessage("Series Instance UID cannot be empty")
            .MaximumLength(64).WithMessage("Series Instance UID exceeds maximum length")
            .Matches(@"^([0-9]+\.)+[0-9]+$").WithMessage("Invalid Series Instance UID format");
    }
}