using FluentValidation;
using TheSSS.DICOMViewer.Domain.Aggregates.StudyAggregate.Entities;

namespace TheSSS.DICOMViewer.Domain.Validation;

public class InstanceValidator : AbstractValidator<Instance>
{
    public InstanceValidator()
    {
        RuleFor(i => i.SopInstanceUid).NotNull().SetValidator(new SopInstanceUidValidator());
        RuleFor(i => i.SOPClassUID).NotEmpty();
        RuleFor(i => i.Rows).GreaterThan((ushort)0);
        RuleFor(i => i.Columns).GreaterThan((ushort)0);
    }
}