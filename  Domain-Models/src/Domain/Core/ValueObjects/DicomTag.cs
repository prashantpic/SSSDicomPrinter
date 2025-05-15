namespace TheSSS.DicomViewer.Domain.Core.ValueObjects
{
    public record DicomTag(ushort Group, ushort Element)
    {
        public override string ToString() => $"({Group:X4},{Element:X4})";
    }
}