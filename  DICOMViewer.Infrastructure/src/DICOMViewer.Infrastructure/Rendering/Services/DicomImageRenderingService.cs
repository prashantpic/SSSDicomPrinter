using System.Threading.Tasks;
using foDICOM.Imaging;
using TheSSS.DICOMViewer.Application.Interfaces.Rendering;
using TheSSS.DICOMViewer.Domain.Models.Rendering;

namespace TheSSS.DICOMViewer.Infrastructure.Rendering.Services
{
    public class DicomImageRenderingService : IDicomImageRenderer
    {
        public async Task<System.Drawing.Bitmap> RenderImageAsync(DicomFile dicomFile, ImageRenderingOptions options)
        {
            return await Task.Run(() => 
            {
                var image = new DICOMImage(dicomFile.Dataset);
                image.WindowCenter = options.WindowCenter;
                image.WindowWidth = options.WindowWidth;
                return image.RenderImage().AsBitmap();
            });
        }
    }
}