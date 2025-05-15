using FluentValidation;
using TheSSS.DICOMViewer.Domain.Aggregates.StudyAggregate.ValueObjects;

namespace TheSSS.DICOMViewer.Domain.Validation;

public class SopInstanceUidValidator : AbstractValidator<string>
{
    public SopInstanceUidValidator()
    {
        RuleFor(uid => uid)
            .NotEmpty().WithMessage("SOP Instance UID cannot be empty")
            .MaximumLength(64).WithMessage("SOP Instance UID exceeds maximum length")
            .Matches(@"^([0-9]+\.)+[0-9]+$").WithMessage("Invalid SOP Instance UID format");
    }
}