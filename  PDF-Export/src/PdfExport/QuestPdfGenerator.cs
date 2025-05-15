using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using System;
using System.Threading;
using System.Threading.Tasks;
using TheSSS.DicomViewer.Pdf.Interfaces;
using TheSSS.DicomViewer.Pdf.Models;
using TheSSS.DicomViewer.Pdf.Internal.Documents;

namespace TheSSS.DicomViewer.Pdf
{
    public class QuestPdfGenerator : IPdfGenerator
    {
        private readonly ILogger<QuestPdfGenerator> _logger;

        public QuestPdfGenerator(ILogger<QuestPdfGenerator> logger)
        {
            _logger = logger;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public Task<byte[]> GeneratePdfAsync(PdfDocumentDataSource documentSource, PdfGenerationOptions options, IProgress<PdfGenerationProgress> progress, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    var pdfDocument = new DicomPdfDocument(documentSource, options, progress, cancellationToken);
                    byte[] pdfBytes = Document.Create(pdfDocument.Compose).GeneratePdf();
                    progress?.Report(new PdfGenerationProgress { PercentageComplete = 100, StatusMessage = "Completed" });
                    return pdfBytes;
                }
                catch (OperationCanceledException)
                {
                    progress?.Report(new PdfGenerationProgress { StatusMessage = "Cancelled" });
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "PDF generation error");
                    progress?.Report(new PdfGenerationProgress { StatusMessage = $"Error: {ex.Message}" });
                    throw;
                }
            }, cancellationToken);
        }
    }
}