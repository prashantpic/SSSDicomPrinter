using SkiaSharp;
using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace TheSSS.DicomViewer.Rendering.Utilities;

public static class DirectXInteropHelper
{
    public static bool IsSupportedOs()
    {
        return Environment.OSVersion.Platform == PlatformID.Win32NT 
            && Environment.OSVersion.Version >= new Version(6, 2);
    }

    public static IntPtr CreateAndShareTexture(GRDirectContext grContext, int width, int height, out SKSurface surface)
    {
        surface = null;
        // Implementation would create shared texture and surface here
        throw new NotImplementedException("DirectX texture sharing not implemented");
    }

    public static void UpdateD3DImageFromSurface(D3DImage d3dImage, SKSurface skSurface, IntPtr sharedTextureHandle)
    {
        d3dImage.Lock();
        try
        {
            d3dImage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, sharedTextureHandle);
            d3dImage.AddDirtyRect(new System.Windows.Int32Rect(0, 0, d3dImage.PixelWidth, d3dImage.PixelHeight));
        }
        finally
        {
            d3dImage.Unlock();
        }
    }
}