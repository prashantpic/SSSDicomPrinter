using QuestPDF.Helpers;
using TheSSS.DicomViewer.Common.Data;

namespace TheSSS.DicomViewer.Pdf.Models
{
    public class PdfGenerationOptions
    {
        public string? Password { get; set; }
        public PdfComplianceLevel ComplianceLevel { get; set; }
        public DicomMetadataEmbeddingOptions? MetadataEmbeddingOptions { get; set; }
        public PageSize DefaultPaperSize { get; set; }
        public int DefaultResolutionDpi { get; set; }
        public bool IsMonochrome { get; set; }
    }
}