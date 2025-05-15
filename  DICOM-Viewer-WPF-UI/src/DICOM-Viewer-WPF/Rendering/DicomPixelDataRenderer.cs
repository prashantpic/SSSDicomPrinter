using SkiaSharp;
using TheSSS.DicomViewer.Presentation.ViewModels;

namespace TheSSS.DicomViewer.Presentation.Rendering
{
    public class DicomPixelDataRenderer : IRenderer<SKCanvas, DicomImageViewModel, SKRect>
    {
        public void Render(SKCanvas canvas, DicomImageViewModel imageViewModel, SKRect destinationRect)
        {
            if (imageViewModel.PixelData == null) return;

            using var bitmap = new SKBitmap(imageViewModel.ImageWidth, imageViewModel.ImageHeight);
            bitmap.Pixels = ConvertPixelData(imageViewModel);
            
            canvas.Save();
            canvas.Translate((float)imageViewModel.PanOffset.X, (float)imageViewModel.PanOffset.Y);
            canvas.Scale((float)imageViewModel.Zoom);
            canvas.DrawBitmap(bitmap, destinationRect);
            canvas.Restore();
        }

        private SKColor[] ConvertPixelData(DicomImageViewModel viewModel)
        {
            // Simplified conversion - actual implementation would handle DICOM specifics
            var pixels = new SKColor[viewModel.ImageWidth * viewModel.ImageHeight];
            for (int i = 0; i < pixels.Length; i++)
            {
                byte value = viewModel.PixelData[i % viewModel.PixelData.Length];
                pixels[i] = new SKColor(value, value, value);
            }
            return pixels;
        }
    }
}