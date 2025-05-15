using SkiaSharp;
using System;
using System.Collections.Generic;
using TheSSS.DicomViewer.Rendering;

namespace TheSSS.DicomViewer.Rendering.Internal.PixelProcessing
{
    public class PixelProcessor : IPixelProcessor
    {
        public SKBitmap ProcessFrameToSkBitmap(RenderableDicomFrame dicomFrame, RenderingOptions options)
        {
            var colorType = DetermineColorType(dicomFrame);
            var bitmap = new SKBitmap(dicomFrame.Width, dicomFrame.Height, colorType, SKAlphaType.Premul);
            
            var pixels = bitmap.GetPixels();
            ProcessPixels(dicomFrame, options, pixels, bitmap.Info);
            bitmap.NotifyPixelsChanged();

            return bitmap;
        }

        private SKColorType DetermineColorType(RenderableDicomFrame frame)
        {
            return frame.PhotometricInterpretation switch
            {
                "MONOCHROME1" or "MONOCHROME2" => SKColorType.Gray8,
                "RGB" or "YBR_FULL" or "PALETTE COLOR" => SKColorType.Bgra8888,
                _ => throw new RenderingException($"Unsupported Photometric Interpretation: {frame.PhotometricInterpretation}")
            };
        }

        private void ProcessPixels(RenderableDicomFrame frame, RenderingOptions options, IntPtr pixels, SKImageInfo info)
        {
            switch (frame.PhotometricInterpretation)
            {
                case "MONOCHROME1":
                case "MONOCHROME2":
                    ProcessMonochrome(frame, options, pixels, info);
                    break;
                case "RGB":
                    ProcessRgb(frame, options, pixels, info);
                    break;
                case "YBR_FULL":
                    ProcessYbr(frame, options, pixels, info);
                    break;
                case "PALETTE COLOR":
                    ProcessPalette(frame, options, pixels, info);
                    break;
            }
        }

        private void ProcessMonochrome(RenderableDicomFrame frame, RenderingOptions options, IntPtr pixels, SKImageInfo info)
        {
            // Implementation for monochrome processing
        }

        private void ProcessRgb(RenderableDicomFrame frame, RenderingOptions options, IntPtr pixels, SKImageInfo info)
        {
            // Implementation for RGB processing
        }

        private void ProcessYbr(RenderableDicomFrame frame, RenderingOptions options, IntPtr pixels, SKImageInfo info)
        {
            // Implementation for YBR conversion
        }

        private void ProcessPalette(RenderableDicomFrame frame, RenderingOptions options, IntPtr pixels, SKImageInfo info)
        {
            // Implementation for palette color
        }
    }
}