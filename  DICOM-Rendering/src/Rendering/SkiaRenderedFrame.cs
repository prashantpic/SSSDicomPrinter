using SkiaSharp;
using System;

namespace TheSSS.DicomViewer.Rendering
{
    public sealed class SkiaRenderedFrame : IDisposable
    {
        public SKImage Image { get; }
        public IntPtr? SharedTextureHandle { get; }

        public SkiaRenderedFrame(SKImage image, IntPtr? sharedTextureHandle = null)
        {
            Image = image;
            SharedTextureHandle = sharedTextureHandle;
        }

        public void Dispose()
        {
            Image?.Dispose();
        }
    }
}