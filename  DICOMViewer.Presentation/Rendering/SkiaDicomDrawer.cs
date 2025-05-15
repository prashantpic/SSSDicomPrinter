using SkiaSharp;
using TheSSS.DICOMViewer.Presentation.Models;

namespace TheSSS.DICOMViewer.Presentation.Rendering
{
    public class SkiaDicomDrawer : ISkiaDicomDrawer
    {
        public void DrawFrame(SKCanvas canvas, DicomFrameData frameData, RenderingOptions options, int width, int height)
        {
            using var bitmap = new SKBitmap(frameData.Width, frameData.Height);
            bitmap.Pixels = ProcessPixelData(frameData, options);
            
            canvas.Save();
            canvas.Translate(options.PanOffsetX, options.PanOffsetY);
            canvas.Scale(options.ZoomFactor);
            canvas.DrawBitmap(bitmap, 0, 0);
            canvas.Restore();
        }

        private SKColor[] ProcessPixelData(DicomFrameData frameData, RenderingOptions options)
        {
            // Simplified pixel processing logic
            var pixels = new SKColor[frameData.Width * frameData.Height];
            for (int i = 0; i < frameData.PixelData.Length; i++)
            {
                byte value = frameData.PixelData[i];
                pixels[i] = new SKColor(value, value, value);
            }
            return pixels;
        }
    }
}