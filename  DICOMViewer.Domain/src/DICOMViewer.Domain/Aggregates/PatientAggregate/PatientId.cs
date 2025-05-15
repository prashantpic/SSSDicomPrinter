namespace TheSSS.DICOMViewer.Domain.Aggregates.PatientAggregate;

public record struct PatientId
{
    public string Value { get; }

    private PatientId(string value) => Value = value;

    public static PatientId Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidDicomIdentifierException("Patient ID cannot be empty", value);

        if (value.Length > 64)
            throw new InvalidDicomIdentifierException("Patient ID exceeds 64 characters", value);

        return new PatientId(value);
    }

    public override string ToString() => Value;

    public static implicit operator string(PatientId id) => id.Value;
}