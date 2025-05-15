using TheSSS.DicomViewer.Domain.Core.Identifiers;

namespace TheSSS.DicomViewer.Domain.Anonymization
{
    public class PixelAnonymizationTemplate
    {
        public PixelAnonymizationTemplateId Id { get; init; }
        public string TemplateName { get; private set; }
        public PixelTemplateContent Definition { get; private set; }

        public PixelAnonymizationTemplate(PixelAnonymizationTemplateId id, string templateName, PixelTemplateContent definition)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            TemplateName = templateName ?? throw new ArgumentNullException(nameof(templateName));
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        }
    }
}