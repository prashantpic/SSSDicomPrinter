using SkiaSharp;
using TheSSS.DicomViewer.Presentation.ViewModels;

namespace TheSSS.DicomViewer.Presentation.Rendering
{
    public class DicomPixelDataRenderer : IRenderer<SKCanvas, DicomImageViewModel, SKRect>
    {
        public void Render(SKCanvas canvas, DicomImageViewModel imageViewModel, SKRect destinationRect)
        {
            canvas.Clear(SKColors.Black);
            using var paint = new SKPaint();
            
            canvas.Translate((float)imageViewModel.PanOffset.X, (float)imageViewModel.PanOffset.Y);
            canvas.Scale((float)imageViewModel.Zoom);
            
            using var bitmap = new SKBitmap(800, 600);
            using var skImage = SKImage.FromBitmap(bitmap);
            canvas.DrawImage(skImage, destinationRect);
        }
    }
}