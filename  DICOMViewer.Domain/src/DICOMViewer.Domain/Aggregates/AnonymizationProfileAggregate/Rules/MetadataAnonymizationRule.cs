using TheSSS.DICOMViewer.Domain.ValueObjects;

namespace TheSSS.DICOMViewer.Domain.Aggregates.AnonymizationProfileAggregate.Rules;

public record MetadataAnonymizationRule
{
    public DicomTagPath DicomTagPath { get; }
    public AnonymizationActionType ActionType { get; }
    public string? ReplacementValue { get; }

    private MetadataAnonymizationRule(DicomTagPath dicomTagPath, AnonymizationActionType actionType, string? replacementValue)
    {
        DicomTagPath = dicomTagPath;
        ActionType = actionType;
        ReplacementValue = replacementValue;
    }

    public static MetadataAnonymizationRule Create(DicomTagPath tagPath, AnonymizationActionType actionType, string? replacementValue = null)
    {
        if (actionType == AnonymizationActionType.ReplaceWithFixedValue && string.IsNullOrEmpty(replacementValue))
            throw new BusinessRuleViolationException("Replacement value required for Replace action");

        return new MetadataAnonymizationRule(tagPath, actionType, replacementValue);
    }
}