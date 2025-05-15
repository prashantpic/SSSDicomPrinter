using FluentValidation;
using TheSSS.DICOMViewer.Domain.Aggregates.StudyAggregate.ValueObjects;

namespace TheSSS.DICOMViewer.Domain.Validation;

public class SeriesInstanceUidValidator : AbstractValidator<SeriesInstanceUid>
{
    public SeriesInstanceUidValidator()
    {
        RuleFor(x => x.Value)
            .NotEmpty().WithMessage("Series Instance UID is required")
            .MaximumLength(64).WithMessage("UID exceeds 64 character limit")
            .Matches(@"^[0-9\.]+$").WithMessage("Invalid UID format - only digits and periods allowed")
            .Must(uid => !uid.StartsWith('.') && !uid.EndsWith('.')).WithMessage("UID cannot start or end with period");
    }
}