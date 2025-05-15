namespace TheSSS.DICOMViewer.Domain.Aggregates.AnonymizationProfileAggregate.Rules;
using TheSSS.DICOMViewer.Domain.ValueObjects;
using TheSSS.DICOMViewer.Domain.Validation;

public readonly record struct MetadataAnonymizationRule
{
    public DicomTagPath DicomTagPath { get; }
    public AnonymizationActionType ActionType { get; }
    public string? ReplacementValue { get; }

    private MetadataAnonymizationRule(DicomTagPath tagPath, AnonymizationActionType actionType, string? replacementValue)
    {
        DicomTagPath = tagPath;
        ActionType = actionType;
        ReplacementValue = replacementValue;
    }

    public static MetadataAnonymizationRule Create(DicomTagPath tagPath, AnonymizationActionType actionType, string? replacementValue)
    {
        var rule = new MetadataAnonymizationRule(tagPath, actionType, replacementValue);
        var validator = new MetadataAnonymizationRuleValidator();
        var result = validator.Validate(rule);
        
        if (!result.IsValid)
            throw new ValidationException(result.Errors);

        return rule;
    }
}