using TheSSS.DicomViewer.Common.Data;

namespace TheSSS.DicomViewer.Pdf.Models
{
    public class PdfImageDataSource
    {
        public byte[] ImageData { get; set; } = [];
        public int ResolutionDpi { get; set; }
        public DicomMetadataCollection? AssociatedDicomMetadata { get; set; }
        public string? Description { get; set; }
    }
}