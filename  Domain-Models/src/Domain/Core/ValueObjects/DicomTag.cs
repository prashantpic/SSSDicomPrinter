namespace TheSSS.DicomViewer.Domain.Core.ValueObjects
{
    public sealed record DicomTag
    {
        public ushort Group { get; }
        public ushort Element { get; }

        public DicomTag(ushort group, ushort element)
        {
            Group = group;
            Element = element;
        }

        public bool Equals(DicomTag? other) => other != null && Group == other.Group && Element == other.Element;
        public override int GetHashCode() => HashCode.Combine(Group, Element);
    }
}