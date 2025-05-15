namespace TheSSS.DICOMViewer.Presentation.Models
{
    public class DicomFrameData
    {
        public byte[] PixelData { get; set; } = [];
        public int Width { get; set; }
        public int Height { get; set; }
        public string PhotometricInterpretation { get; set; } = string.Empty;
        public int BitsAllocated { get; set; }
        public int BitsStored { get; set; }
        public int HighBit { get; set; }
        public int PixelRepresentation { get; set; }
        public int SamplesPerPixel { get; set; }
        public int? PlanarConfiguration { get; set; }
        public double? DefaultWindowCenter { get; set; }
        public double? DefaultWindowWidth { get; set; }
    }
}