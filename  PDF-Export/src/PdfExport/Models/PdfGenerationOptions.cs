using QuestPDF.Helpers;

namespace TheSSS.DicomViewer.Pdf.Models
{
    public class PdfGenerationOptions
    {
        public string? Password { get; set; }
        public PdfComplianceLevel ComplianceLevel { get; set; }
        public DicomMetadataEmbeddingOptions? MetadataEmbeddingOptions { get; set; }
        public PageSize DefaultPaperSize { get; set; } = PageSizes.A4;
        public int DefaultResolutionDpi { get; set; } = 300;
        public bool IsMonochrome { get; set; }
    }
}