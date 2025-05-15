using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using TheSSS.DicomViewer.Pdf.Models;

namespace TheSSS.DicomViewer.Pdf.Internal.Components
{
    internal class ImageRenderComponent : IComponent
    {
        private readonly PdfImageDataSource _imageDataSource;

        public ImageRenderComponent(PdfImageDataSource imageDataSource)
        {
            _imageDataSource = imageDataSource;
        }

        public void Compose(IContainer container)
        {
            if (_imageDataSource.ImageData?.Length > 0)
                container.Image(_imageDataSource.ImageData).FitArea();
        }
    }
}