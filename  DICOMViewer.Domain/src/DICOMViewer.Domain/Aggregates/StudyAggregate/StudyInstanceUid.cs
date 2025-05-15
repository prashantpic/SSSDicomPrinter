namespace TheSSS.DICOMViewer.Domain.Aggregates.StudyAggregate;

public record struct StudyInstanceUid
{
    public string Value { get; }

    private StudyInstanceUid(string value) => Value = value;

    public static StudyInstanceUid Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new InvalidDicomIdentifierException("Study Instance UID cannot be empty", value);

        if (value.Length > 64)
            throw new InvalidDicomIdentifierException("Study Instance UID exceeds 64 characters", value);

        if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[0-9.]+$"))
            throw new InvalidDicomIdentifierException("Study Instance UID contains invalid characters", value);

        return new StudyInstanceUid(value);
    }

    public override string ToString() => Value;

    public static implicit operator string(StudyInstanceUid uid) => uid.Value;
}