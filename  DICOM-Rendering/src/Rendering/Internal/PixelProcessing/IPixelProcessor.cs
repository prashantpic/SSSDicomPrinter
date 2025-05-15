using SkiaSharp;
using TheSSS.DicomViewer.Rendering;

namespace TheSSS.DicomViewer.Rendering.Internal.PixelProcessing;

public interface IPixelProcessor
{
    SKBitmap ProcessFrameToSkBitmap(RenderableDicomFrame dicomFrame, RenderingOptions options);
}