using SkiaSharp;
using TheSSS.DicomViewer.Presentation.ViewModels;

namespace TheSSS.DicomViewer.Presentation.Rendering;

public class DicomPixelDataRenderer : IRenderer
{
    public void Render(object canvasContext, object imageViewModel, object renderParameters)
    {
        if (canvasContext is SKCanvas canvas && 
            imageViewModel is DicomImageViewModel viewModel && 
            renderParameters is SKRect destinationRect)
        {
            // Basic rendering implementation
            using var paint = new SKPaint();
            using var bitmap = new SKBitmap(viewModel.ImageWidth, viewModel.ImageHeight);
            
            if (viewModel.PixelData != null)
            {
                var pixels = bitmap.Pixels;
                for (int i = 0; i < viewModel.PixelData.Length; i++)
                {
                    // Simplified pixel processing
                    byte value = viewModel.PixelData[i];
                    pixels[i] = new SKColor(value, value, value);
                }
                bitmap.Pixels = pixels;
            }
            
            canvas.DrawBitmap(bitmap, destinationRect, paint);
        }
    }
}