namespace TheSSS.DicomViewer.Pdf.Models
{
    public class PdfGenerationProgress
    {
        public int PercentageComplete { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public string StatusMessage { get; set; } = string.Empty;
    }
}