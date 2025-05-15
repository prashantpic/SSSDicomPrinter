namespace TheSSS.DICOMViewer.Domain.Validation;
using FluentValidation;
using TheSSS.DICOMViewer.Domain.Aggregates.AnonymizationProfileAggregate.Rules;

public class MetadataAnonymizationRuleValidator : AbstractValidator<MetadataAnonymizationRule>
{
    public MetadataAnonymizationRuleValidator()
    {
        RuleFor(x => x.DicomTagPath)
            .NotEmpty()
            .WithMessage("DICOM tag path is required");

        RuleFor(x => x.ActionType)
            .IsInEnum()
            .WithMessage("Invalid anonymization action type");

        When(x => x.ActionType == AnonymizationActionType.ReplaceWithFixedValue, () => 
        {
            RuleFor(x => x.ReplacementValue)
                .NotEmpty()
                .WithMessage("Replacement value is required for ReplaceWithFixedValue action");
        });
    }
}