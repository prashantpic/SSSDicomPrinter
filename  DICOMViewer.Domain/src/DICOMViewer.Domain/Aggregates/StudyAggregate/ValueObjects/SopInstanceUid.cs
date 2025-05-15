using TheSSS.DICOMViewer.Domain.Exceptions;
using System.Text.RegularExpressions;

namespace TheSSS.DICOMViewer.Domain.Aggregates.StudyAggregate.ValueObjects;

public record struct SopInstanceUid
{
    private static readonly Regex ValidUidRegex = new(@"^[0-9\.]+$", RegexOptions.Compiled);
    
    public string Value { get; }

    private SopInstanceUid(string value) => Value = value;

    public static SopInstanceUid Create(string uid)
    {
        if (string.IsNullOrWhiteSpace(uid))
            throw new InvalidDicomIdentifierException("SOP Instance UID cannot be empty", uid);

        if (uid.Length > 64)
            throw new InvalidDicomIdentifierException("SOP Instance UID exceeds 64 characters", uid);

        if (!ValidUidRegex.IsMatch(uid))
            throw new InvalidDicomIdentifierException("Invalid SOP Instance UID format", uid);

        return new SopInstanceUid(uid);
    }

    public override string ToString() => Value;
}