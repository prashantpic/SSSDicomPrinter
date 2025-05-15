using TheSSS.DICOMViewer.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace TheSSS.DICOMViewer.Domain.Aggregates.StudyAggregate.ValueObjects;

public record struct SeriesInstanceUid
{
    private static readonly Regex ValidUidRegex = new(@"^[0-9\.]+$", RegexOptions.Compiled);
    
    public string Value { get; }

    private SeriesInstanceUid(string value) => Value = value;

    public static SeriesInstanceUid Create(string uid)
    {
        if (string.IsNullOrWhiteSpace(uid))
            throw new InvalidDicomIdentifierException("Series Instance UID cannot be empty", uid);

        if (uid.Length > 64)
            throw new InvalidDicomIdentifierException("Series Instance UID exceeds 64 characters", uid);

        if (!ValidUidRegex.IsMatch(uid))
            throw new InvalidDicomIdentifierException("Invalid Series Instance UID format", uid);

        return new SeriesInstanceUid(uid);
    }

    public override string ToString() => Value;
}