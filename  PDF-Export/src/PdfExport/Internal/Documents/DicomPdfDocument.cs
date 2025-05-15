using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System;
using System.Threading;
using TheSSS.DicomViewer.Common.Data;
using TheSSS.DicomViewer.Pdf.Models;
using TheSSS.DicomViewer.Pdf.Internal.Components;

namespace TheSSS.DicomViewer.Pdf.Internal.Documents
{
    internal class DicomPdfDocument : IDocument
    {
        private readonly PdfDocumentDataSource _documentSource;
        private readonly PdfGenerationOptions _options;
        private readonly IProgress<PdfGenerationProgress>? _progressReporter;
        private readonly CancellationToken _cancellationToken;

        public DicomPdfDocument(PdfDocumentDataSource documentSource, PdfGenerationOptions options, IProgress<PdfGenerationProgress> progressReporter, CancellationToken cancellationToken)
        {
            _documentSource = documentSource;
            _options = options;
            _progressReporter = progressReporter;
            _cancellationToken = cancellationToken;
        }

        public DocumentMetadata GetMetadata()
        {
            return new DocumentMetadata
            {
                Title = _documentSource.DocumentTitle ?? "DICOM Report",
                PdfACompliance = _options.ComplianceLevel == PdfComplianceLevel.PdfA3b ? PdfAVersion.PdfA3b : PdfAVersion.None,
                DocumentPassword = _options.Password
            };
        }

        public void Compose(IDocumentContainer container)
        {
            if (_documentSource.Pages?.Count == 0)
            {
                container.Page(p => p.Content().Text("No content"));
                return;
            }

            int totalPages = _documentSource.Pages.Count;
            for (int i = 0; i < totalPages; i++)
            {
                _cancellationToken.ThrowIfCancellationRequested();
                var pageData = _documentSource.Pages[i];
                int currentPage = i + 1;

                _progressReporter?.Report(new PdfGenerationProgress
                {
                    PercentageComplete = (int)((double)currentPage / totalPages * 100),
                    CurrentPage = currentPage,
                    TotalPages = totalPages,
                    StatusMessage = $"Processing page {currentPage}"
                });

                container.Page(page =>
                {
                    page.Size(pageData.PageSize ?? _options.DefaultPaperSize, pageData.Orientation ?? PageOrientation.Portrait);
                    page.Content().Padding(25).Column(col =>
                    {
                        if (currentPage == 1 && _options.MetadataEmbeddingOptions?.Mode != MetadataEmbeddingMode.None)
                        {
                            col.Item().Component(new MetadataRenderComponent(_documentSource.GlobalDicomMetadata, _options.MetadataEmbeddingOptions));
                        }

                        foreach (var image in pageData.Images)
                        {
                            col.Item().Component(new ImageRenderComponent(image));
                        }

                        foreach (var text in pageData.Texts)
                        {
                            col.Item().Text(text.Text)
                                .FontSize(text.FontSize)
                                .FontFamily(text.FontFamily ?? Fonts.Arial)
                                .FontColor(Color.Parse(text.Color ?? Colors.Black.ToString()));
                        }
                    });
                });
            }
        }
    }
}