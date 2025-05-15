using FellowOakDicom;
using FellowOakDicom.Imaging;
using System.Drawing;

namespace TheSSS.DICOMViewer.Infrastructure.Rendering.Services
{
    public class DicomImageRenderingService : IDicomImageRenderingService
    {
        private readonly ILoggerAdapter<DicomImageRenderingService> _logger;

        public DicomImageRenderingService(ILoggerAdapter<DicomImageRenderingService> logger)
        {
            _logger = logger;
        }

        public async Task<Bitmap> RenderImageAsync(DicomFile dicomFile, RenderingOptions options)
        {
            try
            {
                var dicomImage = new DicomImage(dicomFile.Dataset);
                var image = dicomImage.RenderImage();
                return image.AsBitmap();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DICOM image rendering failed");
                return null;
            }
        }
    }
}