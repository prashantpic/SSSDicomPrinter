using System;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Rendering
{
    public interface IDicomImageRenderer
    {
        Task InitializeGpuResourcesAsync(IntPtr? windowHandle);
        Task<SkiaRenderedFrame> RenderFrameAsync(RenderableDicomFrame dicomFrame, RenderingOptions options);
        void ReleaseGpuResources();
    }
}