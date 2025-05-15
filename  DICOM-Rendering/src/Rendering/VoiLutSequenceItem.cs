namespace TheSSS.DicomViewer.Rendering
{
    public sealed class VoiLutSequenceItem
    {
        public ushort[] LutDescriptor { get; init; } = default!;
        public byte[] LutData { get; init; } = default!;
    }
}