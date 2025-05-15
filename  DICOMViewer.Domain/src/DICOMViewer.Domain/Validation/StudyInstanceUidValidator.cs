using FluentValidation;
using TheSSS.DICOMViewer.Domain.Aggregates.StudyAggregate;

namespace TheSSS.DICOMViewer.Domain.Validation;

public class StudyInstanceUidValidator : AbstractValidator<StudyInstanceUid>
{
    public StudyInstanceUidValidator()
    {
        RuleFor(uid => uid.Value)
            .NotEmpty()
            .MaximumLength(64)
            .Matches(@"^[0-9.]+$");
    }
}