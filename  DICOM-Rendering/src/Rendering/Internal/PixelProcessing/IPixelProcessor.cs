using SkiaSharp;

namespace TheSSS.DicomViewer.Rendering.Internal.PixelProcessing
{
    internal interface IPixelProcessor
    {
        SKBitmap ProcessFrameToSkBitmap(RenderableDicomFrame dicomFrame, RenderingOptions options);
    }
}