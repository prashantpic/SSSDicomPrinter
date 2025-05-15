namespace TheSSS.DICOMViewer.Domain.Validation;
using FluentValidation;
using TheSSS.DICOMViewer.Domain.Aggregates.AnonymizationProfileAggregate;

public class AnonymizationProfileValidator : AbstractValidator<AnonymizationProfile>
{
    public AnonymizationProfileValidator()
    {
        RuleFor(x => x.ProfileName)
            .NotEmpty()
            .MaximumLength(100);

        RuleForEach(x => x.Rules)
            .SetValidator(new MetadataAnonymizationRuleValidator());
    }
}