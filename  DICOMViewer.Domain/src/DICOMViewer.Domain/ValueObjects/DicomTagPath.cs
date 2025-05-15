namespace TheSSS.DICOMViewer.Domain.ValueObjects;
public readonly record struct DicomTagPath
{
    public ushort Group { get; }
    public ushort Element { get; }

    private DicomTagPath(ushort group, ushort element)
    {
        Group = group;
        Element = element;
    }

    public static DicomTagPath Create(ushort group, ushort element)
    {
        return new DicomTagPath(group, element);
    }

    public static DicomTagPath Parse(string tagString)
    {
        if (string.IsNullOrWhiteSpace(tagString))
            throw new InvalidDicomIdentifierException(tagString, "Tag string cannot be empty");

        var cleaned = tagString.Trim('(', ')').Replace(",", " ");
        var parts = cleaned.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 2 || 
            !ushort.TryParse(parts[0], System.Globalization.NumberStyles.HexNumber, null, out var group) ||
            !ushort.TryParse(parts[1], System.Globalization.NumberStyles.HexNumber, null, out var element))
        {
            throw new InvalidDicomIdentifierException(tagString, "Invalid DICOM tag format");
        }

        return new DicomTagPath(group, element);
    }

    public override string ToString() => $"({Group:X4},{Element:X4})";

    public override int GetHashCode() => HashCode.Combine(Group, Element);
}