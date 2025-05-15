namespace TheSSS.DicomViewer.Rendering
{
    public sealed class RenderableDicomFrame
    {
        public Memory<byte> PixelData { get; init; }
        public int Width { get; init; }
        public int Height { get; init; }
        public int BitsAllocated { get; init; }
        public int BitsStored { get; init; }
        public int HighBit { get; init; }
        public ushort PixelRepresentation { get; init; }
        public string PhotometricInterpretation { get; init; } = string.Empty;
        public ushort SamplesPerPixel { get; init; }
        public ushort? PlanarConfiguration { get; init; }
        public double RescaleSlope { get; init; } = 1.0;
        public double RescaleIntercept { get; init; } = 0.0;
        public ushort[]? PaletteColorLutDescriptor { get; init; }
        public byte[]? PaletteColorLutData { get; init; }
    }
}