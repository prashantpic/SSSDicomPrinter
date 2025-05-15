using TheSSS.DicomViewer.Common.Models.Imaging;

namespace TheSSS.DicomViewer.Common.Models.Imaging
{
    public class WindowLevelInput
    {
        public double WindowWidth { get; set; }
        public double WindowCenter { get; set; }
        public PixelDataDescriptor PixelDescriptor { get; set; }
    }
}