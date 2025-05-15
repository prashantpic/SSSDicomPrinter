using FluentValidation;
using TheSSS.DICOMViewer.Domain.Aggregates.AnonymizationProfileAggregate;

namespace TheSSS.DICOMViewer.Domain.Validation;

public class AnonymizationProfileValidator : AbstractValidator<AnonymizationProfile>
{
    public AnonymizationProfileValidator()
    {
        RuleFor(p => p.ProfileName).NotEmpty().MaximumLength(100);
        RuleForEach(p => p.Rules).SetValidator(new MetadataAnonymizationRuleValidator());
    }
}