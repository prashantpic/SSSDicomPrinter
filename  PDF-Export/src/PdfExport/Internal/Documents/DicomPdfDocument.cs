using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System;
using System.Threading;
using TheSSS.DicomViewer.Common.Data;
using TheSSS.DicomViewer.Pdf.Internal.Components;
using TheSSS.DicomViewer.Pdf.Models;

namespace TheSSS.DicomViewer.Pdf.Internal.Documents
{
    internal class DicomPdfDocument : IDocument
    {
        private readonly PdfDocumentDataSource _documentSource;
        private readonly PdfGenerationOptions _options;
        private readonly IProgress<PdfGenerationProgress>? _progressReporter;
        private readonly CancellationToken _cancellationToken;

        public DicomPdfDocument(PdfDocumentDataSource documentSource, PdfGenerationOptions options, IProgress<PdfGenerationProgress>? progressReporter, CancellationToken cancellationToken)
        {
            _documentSource = documentSource;
            _options = options;
            _progressReporter = progressReporter;
            _cancellationToken = cancellationToken;
        }

        public DocumentMetadata GetMetadata()
        {
            var metadata = DocumentMetadata.Default;
            metadata.Title = _documentSource.DocumentTitle ?? "DICOM Report";
            
            if (_options.ComplianceLevel == PdfComplianceLevel.PdfA3b)
                metadata.PdfACompliance = PdfAVersion.PdfA3b;
            
            if (!string.IsNullOrEmpty(_options.Password))
                metadata.DocumentPassword = _options.Password;

            return metadata;
        }

        public void Compose(IDocumentContainer container)
        {
            int totalPages = _documentSource.Pages.Count;
            for (int i = 0; i < totalPages; i++)
            {
                _cancellationToken.ThrowIfCancellationRequested();
                var pageData = _documentSource.Pages[i];
                
                container.Page(page =>
                {
                    page.Size(pageData.PageSize ?? _options.DefaultPaperSize, pageData.Orientation ?? PageOrientation.Portrait);
                    page.Content().Column(column =>
                    {
                        if (i == 0 && _options.MetadataEmbeddingOptions?.Mode != MetadataEmbeddingMode.None)
                            column.Item().Component(new MetadataRenderComponent(_documentSource.GlobalDicomMetadata, _options.MetadataEmbeddingOptions));

                        foreach (var image in pageData.Images)
                        {
                            column.Item().Component(new ImageRenderComponent(image));
                            if (_options.MetadataEmbeddingOptions?.Mode != MetadataEmbeddingMode.None)
                                column.Item().Component(new MetadataRenderComponent(image.AssociatedDicomMetadata, _options.MetadataEmbeddingOptions));
                        }

                        foreach (var text in pageData.Texts)
                        {
                            column.Item().Text(text.Text)
                                .FontSize(text.FontSize)
                                .FontFamily(text.FontFamily ?? Fonts.Arial)
                                .FontColor(Color.Parse(text.Color ?? Colors.Black.ToString()));
                        }
                    });
                });

                _progressReporter?.Report(new PdfGenerationProgress
                {
                    PercentageComplete = (int)((i + 1) / (double)totalPages * 100),
                    CurrentPage = i + 1,
                    TotalPages = totalPages,
                    StatusMessage = $"Processed page {i + 1} of {totalPages}"
                });
            }
        }
    }
}