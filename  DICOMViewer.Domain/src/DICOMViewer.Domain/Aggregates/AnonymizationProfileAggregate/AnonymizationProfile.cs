using TheSSS.DICOMViewer.Domain.Validation;
using TheSSS.DICOMViewer.Domain.Exceptions;

namespace TheSSS.DICOMViewer.Domain.Aggregates.AnonymizationProfileAggregate;

public class AnonymizationProfile
{
    public AnonymizationProfileId ProfileId { get; private set; }
    public string ProfileName { get; private set; }
    public string? ProfileDescription { get; private set; }
    private readonly List<MetadataAnonymizationRule> _rules = new();
    public IReadOnlyCollection<MetadataAnonymizationRule> Rules => _rules.AsReadOnly();

    private AnonymizationProfile(AnonymizationProfileId profileId, string profileName, string? profileDescription)
    {
        ProfileId = profileId;
        ProfileName = profileName;
        ProfileDescription = profileDescription;
    }

    public static AnonymizationProfile Create(string profileName, string? profileDescription = null)
    {
        var profile = new AnonymizationProfile(AnonymizationProfileId.New(), profileName, profileDescription);
        
        var validator = new AnonymizationProfileValidator();
        var result = validator.Validate(profile);
        
        if (!result.IsValid)
        {
            throw new BusinessRuleViolationException($"Anonymization profile validation failed: {string.Join(", ", result.Errors)}");
        }

        return profile;
    }

    public void AddRule(MetadataAnonymizationRule rule)
    {
        if (_rules.Any(r => r.DicomTagPath.Equals(rule.DicomTagPath)))
            throw new BusinessRuleViolationException("Rule for this DICOM tag already exists");

        _rules.Add(rule);
    }
}