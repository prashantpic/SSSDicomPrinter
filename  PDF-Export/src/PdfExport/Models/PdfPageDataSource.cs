using System.Collections.Generic;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace TheSSS.DicomViewer.Pdf.Models
{
    public class PdfPageDataSource
    {
        public PageSize? PageSize { get; set; }
        public PageOrientation? Orientation { get; set; }
        public IReadOnlyList<PdfImageDataSource> Images { get; set; } = new List<PdfImageDataSource>();
        public IReadOnlyList<PdfTextDataSource> Texts { get; set; } = new List<PdfTextDataSource>();
    }
}