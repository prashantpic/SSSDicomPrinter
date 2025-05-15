using QuestPDF.Infrastructure;
using QuestPDF.Fluent;
using TheSSS.DICOMViewer.Domain.Models;

namespace TheSSS.DICOMViewer.Infrastructure.OutputGeneration.Services
{
    public class PdfGenerationService : IPdfGenerationService
    {
        private readonly ILoggerAdapter<PdfGenerationService> _logger;

        public PdfGenerationService(ILoggerAdapter<PdfGenerationService> logger)
        {
            _logger = logger;
        }

        public async Task GeneratePdfAsync(PdfContent content, PdfGenerationOptions options, Stream outputStream)
        {
            try
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Content().Column(column =>
                        {
                            foreach (var section in content.Sections)
                            {
                                column.Item().Text(section.Title);
                                column.Item().Image(section.ImageData);
                            }
                        });
                    });
                });

                document.GeneratePdf(outputStream);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "PDF generation failed");
            }
        }
    }
}