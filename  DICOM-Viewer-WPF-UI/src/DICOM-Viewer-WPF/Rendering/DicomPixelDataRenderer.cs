using SkiaSharp;
using TheSSS.DicomViewer.Presentation.Rendering;
using TheSSS.DicomViewer.Presentation.ViewModels;

namespace TheSSS.DicomViewer.Presentation.Rendering;

public class DicomPixelDataRenderer : IRenderer<SKCanvas, DicomImageViewModel, SKRect>
{
    public void Render(SKCanvas canvas, DicomImageViewModel imageViewModel, SKRect destinationRect)
    {
        if (imageViewModel.PixelData == null || imageViewModel.PixelData.Length == 0)
            return;

        using var bitmap = new SKBitmap(imageViewModel.ImageWidth, imageViewModel.ImageHeight);
        var pixels = bitmap.GetPixels();
        
        ApplyWindowing(imageViewModel, pixels);
        ApplyPhotometricInterpretation(imageViewModel, pixels);

        canvas.Save();
        canvas.Translate(imageViewModel.PanOffset.X, imageViewModel.PanOffset.Y);
        canvas.Scale((float)imageViewModel.Zoom);
        canvas.DrawBitmap(bitmap, destinationRect);
        canvas.Restore();
    }

    private void ApplyWindowing(DicomImageViewModel vm, IntPtr pixels)
    {
        // Placeholder for window width/level calculation
        // Actual implementation would convert pixel data based on DICOM parameters
    }

    private void ApplyPhotometricInterpretation(DicomImageViewModel vm, IntPtr pixels)
    {
        // Placeholder for photometric interpretation handling
        // Would convert pixel data based on MONOCHROME1/MONOCHROME2
    }
}