namespace TheSSS.DICOMViewer.Domain.Aggregates.AnonymizationProfileAggregate.Rules;

public enum AnonymizationActionType
{
    Remove,
    ReplaceWithFixedValue,
    Hash,
    DateOffset,
    Clean,
    Keep
}