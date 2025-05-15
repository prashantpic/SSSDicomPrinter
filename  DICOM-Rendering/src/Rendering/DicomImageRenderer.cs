using SkiaSharp;
using TheSSS.DicomViewer.Rendering.Internal.Gpu;
using TheSSS.DicomViewer.Rendering.Internal.PixelProcessing;
using TheSSS.DicomViewer.Rendering.Utilities;

namespace TheSSS.DicomViewer.Rendering;

public sealed class DicomImageRenderer : IDicomImageRenderer, IDisposable
{
    private readonly ISkiaDirectXManager _skiaDirectXManager;
    private readonly IPixelProcessor _pixelProcessor;
    private bool _disposed;

    public DicomImageRenderer(ISkiaDirectXManager skiaDirectXManager, IPixelProcessor pixelProcessor)
    {
        _skiaDirectXManager = skiaDirectXManager ?? throw new ArgumentNullException(nameof(skiaDirectXManager));
        _pixelProcessor = pixelProcessor ?? throw new ArgumentNullException(nameof(pixelProcessor));
    }

    public async Task InitializeGpuResourcesAsync(IntPtr? windowHandle)
    {
        try
        {
            await _skiaDirectXManager.InitializeAsync(windowHandle);
        }
        catch (Exception ex)
        {
            throw new RenderingException("Failed to initialize GPU resources", ex);
        }
    }

    public async Task<SkiaRenderedFrame> RenderFrameAsync(RenderableDicomFrame dicomFrame, RenderingOptions options)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(DicomImageRenderer));
        
        try
        {
            using var processedBitmap = _pixelProcessor.ProcessFrameToSkBitmap(dicomFrame, options);
            var grContext = _skiaDirectXManager.GetContext();
            
            using var surface = _skiaDirectXManager.CreateRenderTargetSurface(processedBitmap.Width, processedBitmap.Height);
            var canvas = surface.Canvas;
            
            ApplyTransformations(canvas, options, processedBitmap.Width, processedBitmap.Height);
            
            canvas.DrawBitmap(processedBitmap, 0, 0);
            canvas.Flush();
            grContext.Submit();
            
            var snapshot = surface.Snapshot();
            return new SkiaRenderedFrame(snapshot, null);
        }
        catch (Exception ex)
        {
            throw new RenderingException("Failed to render DICOM frame", ex);
        }
    }

    public void ReleaseGpuResources()
    {
        _skiaDirectXManager.Shutdown();
    }

    public void Dispose()
    {
        if (_disposed) return;
        ReleaseGpuResources();
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    private static void ApplyTransformations(SKCanvas canvas, RenderingOptions options, int width, int height)
    {
        canvas.Translate(width / 2f, height / 2f);
        
        if (options.FlipHorizontal)
            canvas.Scale(-1, 1);
        
        if (options.FlipVertical)
            canvas.Scale(1, -1);
        
        canvas.RotateDegrees(options.RotationDegrees);
        canvas.Scale(options.ScaleFactor);
        
        canvas.Translate(-width / 2f, -height / 2f);
    }
}