using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TheSSS.DICOMViewer.Application.Interfaces.Output;
using System.IO;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Infrastructure.OutputGeneration.Services
{
    public class PdfGenerationService : IPdfGenerator
    {
        public async Task GeneratePdfAsync(PdfDocumentContent content, PdfGenerationOptions options, Stream outputStream)
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.Content().Column(col =>
                    {
                        foreach (var section in content.Sections)
                        {
                            col.Item().Text(section.Title);
                            col.Item().Image(section.ImageData);
                        }
                    });
                });
            });

            document.GeneratePdf(outputStream);
            await Task.CompletedTask;
        }
    }
}