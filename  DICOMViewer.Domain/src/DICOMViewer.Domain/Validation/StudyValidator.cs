using FluentValidation;
using TheSSS.DICOMViewer.Domain.Aggregates.StudyAggregate;

namespace TheSSS.DICOMViewer.Domain.Validation;

public class StudyValidator : AbstractValidator<Study>
{
    public StudyValidator()
    {
        RuleFor(s => s.StudyInstanceUid).NotNull().SetValidator(new StudyInstanceUidValidator());
        RuleFor(s => s.PatientId).NotNull();
        RuleFor(s => s.StudyDate).LessThanOrEqualTo(DateTime.Today);
        RuleFor(s => s.AccessionNumber).MaximumLength(16);
        RuleForEach(s => s.Series).SetValidator(new SeriesValidator());
    }
}