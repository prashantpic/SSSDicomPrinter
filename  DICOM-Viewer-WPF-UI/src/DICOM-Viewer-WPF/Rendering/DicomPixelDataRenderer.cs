using SkiaSharp;
using TheSSS.DicomViewer.Presentation.ViewModels;

namespace TheSSS.DicomViewer.Presentation.Rendering
{
    public class DicomPixelDataRenderer : IRenderer
    {
        public void Render(SKCanvas canvas, DicomImageViewModel imageViewModel, SKRect destinationRect)
        {
            if (imageViewModel.PixelData == null || imageViewModel.ImageWidth == 0 || imageViewModel.ImageHeight == 0)
                return;

            var info = new SKImageInfo(imageViewModel.ImageWidth, imageViewModel.ImageHeight, SKColorType.Gray8, SKAlphaType.Opaque);
            using var bitmap = new SKBitmap(info);
            bitmap.SetPixels(imageViewModel.PixelData);

            using var paint = new SKPaint();
            if (imageViewModel.PhotometricInterpretation == "MONOCHROME2")
            {
                float scale = 255.0f / (float)imageViewModel.WindowWidth;
                float offset = (float)((imageViewModel.WindowLevel - imageViewModel.WindowWidth / 2) * scale);
                
                paint.ColorFilter = SKColorFilter.CreateTable(new byte[256], new byte[256], new byte[256], 
                    CreateLookupTable(scale, offset));
            }

            canvas.Save();
            canvas.Translate((float)imageViewModel.PanOffsetX, (float)imageViewModel.PanOffsetY);
            canvas.Scale((float)imageViewModel.ZoomLevel);
            canvas.DrawBitmap(bitmap, destinationRect, paint);
            canvas.Restore();
        }

        private byte[] CreateLookupTable(float scale, float offset)
        {
            byte[] table = new byte[256];
            for (int i = 0; i < 256; i++)
            {
                float val = (i * scale) - offset;
                table[i] = (byte)Math.Clamp(val, 0, 255);
            }
            return table;
        }
    }
}