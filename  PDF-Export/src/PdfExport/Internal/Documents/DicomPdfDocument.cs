using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Drawing;
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
            {
                metadata.PdfACompliance = PdfAVersion.PdfA3b;
            }

            if (!string.IsNullOrEmpty(_options.Password))
            {
                metadata.DocumentPassword = _options.Password;
                metadata.Permissions = DocumentPermissions.AllowPrinting | DocumentPermissions.AllowCopying;
            }

            return metadata;
        }

        public void Compose(IDocumentContainer container)
        {
            if (_documentSource.Pages == null || _documentSource.Pages.Count == 0)
            {
                container.Page(page => page.Content().Text("No content provided").AlignCenter());
                _progressReporter?.Report(new PdfGenerationProgress { PercentageComplete = 100 });
                return;
            }

            int totalPages = _documentSource.Pages.Count;
            for (int i = 0; i < totalPages; i++)
            {
                _cancellationToken.ThrowIfCancellationRequested();
                var pageData = _documentSource.Pages[i];
                int currentPage = i + 1;

                container.Page(page =>
                {
                    page.Size(pageData.PageSize ?? _options.DefaultPaperSize, 
                             pageData.Orientation ?? PageOrientation.Portrait);
                    
                    page.Content().Padding(25).Column(column =>
                    {
                        column.Spacing(10);

                        if (currentPage == 1 && _options.MetadataEmbeddingOptions?.Mode != MetadataEmbeddingMode.None && 
                            _documentSource.GlobalDicomMetadata != null)
                        {
                            column.Item().Component(new MetadataRenderComponent(_documentSource.GlobalDicomMetadata, 
                                                  _options.MetadataEmbeddingOptions));
                        }

                        foreach (var image in pageData.Images)
                        {
                            _cancellationToken.ThrowIfCancellationRequested();
                            column.Item().Component(new ImageRenderComponent(image));

                            if (_options.MetadataEmbeddingOptions?.Mode != MetadataEmbeddingMode.None && 
                                image.AssociatedDicomMetadata != null)
                            {
                                column.Item().Component(new MetadataRenderComponent(image.AssociatedDicomMetadata, 
                                                      _options.MetadataEmbeddingOptions));
                            }
                        }

                        foreach (var text in pageData.Texts)
                        {
                            _cancellationToken.ThrowIfCancellationRequested();
                            column.Item().Text(text.Text)
                                .FontSize(text.FontSize)
                                .FontFamily(text.FontFamily ?? Fonts.Arial)
                                .FontColor(Color.Parse(text.Color ?? Colors.Black.ToString()));
                        }
                    });

                    _progressReporter?.Report(new PdfGenerationProgress
                    {
                        PercentageComplete = (int)(((double)currentPage / totalPages) * 100),
                        CurrentPage = currentPage,
                        TotalPages = totalPages
                    });
                });
            }
        }
    }
}