using FluentValidation;
using TheSSS.DICOMViewer.Domain.Aggregates.PatientAggregate;

namespace TheSSS.DICOMViewer.Domain.Validation;

public class PatientValidator : AbstractValidator<Patient>
{
    public PatientValidator()
    {
        RuleFor(p => p.PatientId).NotNull().SetValidator(new PatientIdValidator());
        RuleFor(p => p.PatientName).NotEmpty().MaximumLength(64);
        RuleFor(p => p.PatientBirthDate).LessThanOrEqualTo(DateTime.Today);
        RuleFor(p => p.PatientSex).Must(sex => sex == null || new[] { "M", "F", "O" }.Contains(sex));
    }
}