namespace TheSSS.DicomViewer.Common.Models.Imaging
{
    public class PixelDataDescriptor
    {
        public int BitsAllocated { get; set; }
        public int BitsStored { get; set; }
        public int HighBit { get; set; }
        public double RescaleSlope { get; set; } = 1.0;
        public double RescaleIntercept { get; set; } = 0.0;
        public bool IsSigned { get; set; }
    }
}