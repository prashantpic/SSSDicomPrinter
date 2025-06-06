namespace TheSSS.DicomViewer.Pdf.Models
{
    public class PdfTextDataSource
    {
        public string Text { get; set; } = string.Empty;
        public string? FontFamily { get; set; }
        public float FontSize { get; set; } = 12;
        public string? Color { get; set; }
    }
}