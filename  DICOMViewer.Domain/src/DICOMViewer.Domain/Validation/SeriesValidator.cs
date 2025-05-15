using FluentValidation;
using TheSSS.DICOMViewer.Domain.Aggregates.StudyAggregate.Entities;

namespace TheSSS.DICOMViewer.Domain.Validation;

public class SeriesValidator : AbstractValidator<Series>
{
    public SeriesValidator()
    {
        RuleFor(s => s.SeriesInstanceUid).NotNull().SetValidator(new SeriesInstanceUidValidator());
        RuleFor(s => s.Modality).NotEmpty().MaximumLength(16);
        RuleForEach(s => s.Instances).SetValidator(new InstanceValidator());
    }
}