namespace TheSSS.DICOMViewer.Domain.Aggregates.AnonymizationProfileAggregate;

public record struct AnonymizationProfileId
{
    public Guid Value { get; }

    private AnonymizationProfileId(Guid value) => Value = value;

    public static AnonymizationProfileId New() => new(Guid.NewGuid());

    public static AnonymizationProfileId Create(Guid value)
    {
        if (value == Guid.Empty)
            throw new BusinessRuleViolationException("Invalid profile ID");
        
        return new AnonymizationProfileId(value);
    }

    public override string ToString() => Value.ToString();
}