using SkiaSharp;
using TheSSS.DICOMViewer.Presentation.Models;

namespace TheSSS.DICOMViewer.Presentation.Rendering
{
    public interface ISkiaDicomDrawer
    {
        void DrawFrame(SKCanvas canvas, DicomFrameData frameData, RenderingOptions options, int width, int height);
    }
}