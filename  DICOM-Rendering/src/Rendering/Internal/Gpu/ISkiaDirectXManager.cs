using SkiaSharp;
using System;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Rendering.Internal.Gpu
{
    internal interface ISkiaDirectXManager : IDisposable
    {
        Task InitializeAsync(IntPtr? windowHandle);
        GRContext GetContext();
        SKSurface CreateRenderTargetSurface(int width, int height);
        SKSurface CreateRenderTargetSurfaceFromTexture(IntPtr directXTextureHandle, int width, int height, SKColorType pixelConfig);
        void Shutdown();
    }
}