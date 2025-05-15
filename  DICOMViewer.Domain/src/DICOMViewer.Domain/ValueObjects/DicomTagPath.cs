using DICOMViewer.Domain.Exceptions;

namespace DICOMViewer.Domain.ValueObjects
{
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

        public static DicomTagPath Create(string tagString)
        {
            try
            {
                var cleaned = tagString.Trim().TrimStart('(').TrimEnd(')');
                var parts = cleaned.Split(',');
                return new DicomTagPath(
                    Convert.ToUInt16(parts[0], 16),
                    Convert.ToUInt16(parts[1], 16)
                );
            }
            catch
            {
                throw new InvalidDicomIdentifierException(
                    tagString, 
                    "Invalid DICOM tag format. Expected (gggg,eeee) hexadecimal format"
                );
            }
        }

        public override string ToString() => $"({Group:X4},{Element:X4})";

        public bool Equals(DicomTagPath other) => 
            Group == other.Group && Element == other.Element;

        public override int GetHashCode() => HashCode.Combine(Group, Element);
    }
}