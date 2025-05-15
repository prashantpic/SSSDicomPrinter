using System.Text.RegularExpressions;

namespace TheSSS.DICOMViewer.Domain.ValueObjects;

public record struct DicomTagPath(ushort Group, ushort Element)
{
    private static readonly Regex TagPattern = new(@"^\(([0-9A-Fa-f]{4}),([0-9A-Fa-f]{4})\)$");
    
    public static DicomTagPath Parse(string input)
    {
        var match = TagPattern.Match(input);
        if (!match.Success)
            throw new InvalidDicomIdentifierException("Invalid DICOM tag format", input);

        return new DicomTagPath(
            Convert.ToUInt16(match.Groups[1].Value, 16),
            Convert.ToUInt16(match.Groups[2].Value, 16)
        );
    }

    public override string ToString() => $"({Group:X4},{Element:X4})";
}