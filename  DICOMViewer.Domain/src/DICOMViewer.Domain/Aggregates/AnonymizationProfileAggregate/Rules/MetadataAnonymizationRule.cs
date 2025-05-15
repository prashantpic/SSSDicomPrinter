using FluentValidation;
using TheSSS.DICOMViewer.Domain.Validation;

namespace TheSSS.DICOMViewer.Domain.Aggregates.AnonymizationProfileAggregate.Rules;

public record MetadataAnonymizationRule(
    DicomTagPath DicomTagPath,
    AnonymizationActionType ActionType,
    string? ReplacementValue = null)
{
    public static MetadataAnonymizationRule Create(
        DicomTagPath tagPath,
        AnonymizationActionType actionType,
        string? replacementValue = null)
    {
        var rule = new MetadataAnonymizationRule(tagPath, actionType, replacementValue);
        var validator = new MetadataAnonymizationRuleValidator();
        validator.ValidateAndThrow(rule);
        return rule;
    }
}