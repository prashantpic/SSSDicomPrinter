using System.Threading;
using System.Threading.Tasks;
using TheSSS.DicomViewer.Pdf.Models;

namespace TheSSS.DicomViewer.Pdf.Interfaces
{
    public interface IPdfGenerator
    {
        Task<byte[]> GeneratePdfAsync(PdfDocumentDataSource documentSource, PdfGenerationOptions options, IProgress<PdfGenerationProgress> progress, CancellationToken cancellationToken);
    }
}