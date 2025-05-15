using SkiaSharp;
using TheSSS.DicomViewer.Presentation.ViewModels;

namespace TheSSS.DicomViewer.Presentation.Rendering
{
    public class DicomPixelDataRenderer : IRenderer<SKCanvas, DicomImageViewModel, SKRect>
    {
        public void Render(SKCanvas canvas, DicomImageViewModel dataSource, SKRect renderParameters)
        {
            if (dataSource.PixelData == null) return;

            using var bitmap = new SKBitmap(dataSource.ImageWidth, dataSource.ImageHeight);
            bitmap.Pixels = ConvertPixelData(dataSource.PixelData, dataSource.WindowWidth, dataSource.WindowLevel);
            
            canvas.Save();
            canvas.Translate((float)dataSource.PanOffset.X, (float)dataSource.PanOffset.Y);
            canvas.Scale((float)dataSource.Zoom);
            canvas.DrawBitmap(bitmap, renderParameters);
            canvas.Restore();
        }

        private static SKColor[] ConvertPixelData(byte[] pixelData, double ww, double wl)
        {
            // Simplified pixel conversion - actual implementation would handle DICOM specifics
            var colors = new SKColor[pixelData.Length];
            for (int i = 0; i < pixelData.Length; i++)
            {
                byte value = ApplyWindowLevel(pixelData[i], ww, wl);
                colors[i] = new SKColor(value, value, value);
            }
            return colors;
        }

        private static byte ApplyWindowLevel(byte pixelValue, double ww, double wl)
        {
            double min = wl - ww / 2;
            double max = wl + ww / 2;
            double normalized = (pixelValue - min) / (max - min);
            return (byte)Math.Clamp(normalized * 255, 0, 255);
        }
    }
}