using FluentValidation;
using TheSSS.DICOMViewer.Domain.Aggregates.PatientAggregate;

namespace TheSSS.DICOMViewer.Domain.Validation;

public class PatientIdValidator : AbstractValidator<PatientId>
{
    public PatientIdValidator()
    {
        RuleFor(id => id.Value)
            .NotEmpty()
            .MaximumLength(64)
            .Matches(@"^[^\x00-\x1F]*$");
    }
}