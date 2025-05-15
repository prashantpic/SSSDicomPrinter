using TheSSS.DicomViewer.Common.Data;
using TheSSS.DicomViewer.Pdf.Models;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Pdf.Interfaces
{
    public interface IPdfGenerator
    {
        Task<byte[]> GeneratePdfAsync(
            PdfDocumentDataSource documentSource,
            PdfGenerationOptions options,
            IProgress<PdfGenerationProgress>? progress,
            CancellationToken cancellationToken);
    }
}