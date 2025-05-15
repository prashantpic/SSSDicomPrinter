using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System;
using System.Threading;
using System.Threading.Tasks;
using TheSSS.DicomViewer.Pdf.Interfaces;
using TheSSS.DicomViewer.Pdf.Internal.Documents;
using TheSSS.DicomViewer.Pdf.Models;

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

        public async Task<byte[]> GeneratePdfAsync(PdfDocumentDataSource documentSource, PdfGenerationOptions options, IProgress<PdfGenerationProgress> progress, CancellationToken cancellationToken)
        {
            return await Task.Run(() =>
            {
                var document = new DicomPdfDocument(documentSource, options, progress, cancellationToken);
                return Document.Create(container => document.Compose(container)).GeneratePdf();
            }, cancellationToken);
        }
    }
}