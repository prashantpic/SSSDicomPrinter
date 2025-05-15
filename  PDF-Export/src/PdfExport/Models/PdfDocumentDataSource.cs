using System.Collections.Generic;
using TheSSS.DicomViewer.Common.Data;

namespace TheSSS.DicomViewer.Pdf.Models
{
    public class PdfDocumentDataSource
    {
        public IReadOnlyList<PdfPageDataSource> Pages { get; set; } = new List<PdfPageDataSource>();
        public string? DocumentTitle { get; set; }
        public DicomMetadataCollection? GlobalDicomMetadata { get; set; }
    }
}