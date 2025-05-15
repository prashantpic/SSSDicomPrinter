namespace TheSSS.DICOMViewer.Domain.Aggregates.AnonymizationProfileAggregate;
public readonly record struct AnonymizationProfileId
{
    public Guid Value { get; }

    private AnonymizationProfileId(Guid value) => Value = value;

    public static AnonymizationProfileId Create() => new(Guid.NewGuid());
}