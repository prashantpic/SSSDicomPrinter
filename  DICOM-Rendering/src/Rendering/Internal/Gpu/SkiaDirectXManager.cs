using SharpDX.Direct3D11;
using SkiaSharp;
using System;
using System.Threading.Tasks;
using TheSSS.DicomViewer.Rendering;

namespace TheSSS.DicomViewer.Rendering.Internal.Gpu
{
    public sealed class SkiaDirectXManager : ISkiaDirectXManager
    {
        private Device _d3dDevice;
        private DeviceContext _d3dContext;
        private GRContext _grContext;
        private bool _disposed;

        public async Task InitializeAsync(IntPtr? windowHandle)
        {
            try
            {
                var creationFlags = DeviceCreationFlags.BgraSupport;
                if (FeatureFlags.EnableDirectXDebugLayer)
                    creationFlags |= DeviceCreationFlags.Debug;

                _d3dDevice = new Device(SharpDX.Direct3D.DriverType.Hardware, creationFlags);
                _d3dContext = _d3dDevice.ImmediateContext;
                
                var grInterface = GRGlInterface.CreateDirect3DInterface(_d3dDevice.NativePointer);
                _grContext = GRContext.CreateDirect3D(grInterface, _d3dDevice.NativePointer, _d3dContext.NativePointer);
            }
            catch (Exception ex)
            {
                throw new RenderingException("Failed to initialize DirectX resources", ex);
            }
        }

        public GRContext GetContext()
        {
            return _grContext ?? throw new RenderingException("GPU resources not initialized");
        }

        public SKSurface CreateRenderTargetSurface(int width, int height)
        {
            var textureDesc = new Texture2DDescription
            {
                Width = width,
                Height = height,
                MipLevels = 1,
                ArraySize = 1,
                Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.RenderTarget | BindFlags.ShaderResource,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            };

            using var texture = new Texture2D(_d3dDevice, textureDesc);
            return SKSurface.Create(_grContext, new GRBackendRenderTarget(
                width, height, 0, 0,
                new GRDirect3DTextureInfo(texture.NativePointer, GRDirect3DTextureResourceInfoFlags.RenderTarget)));
        }

        public SKSurface CreateRenderTargetSurfaceFromTexture(IntPtr directXTextureHandle, int width, int height, SKColorType pixelConfig)
        {
            var textureInfo = new GRDirect3DTextureInfo(directXTextureHandle, GRDirect3DTextureResourceInfoFlags.RenderTarget);
            return SKSurface.Create(_grContext, new GRBackendRenderTarget(width, height, 0, 0, textureInfo));
        }

        public void Shutdown()
        {
            _grContext?.Dispose();
            _d3dContext?.Dispose();
            _d3dDevice?.Dispose();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Shutdown();
                _disposed = true;
            }
        }
    }
}