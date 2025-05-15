using TheSSS.DicomViewer.Domain.Core.Identifiers;

namespace TheSSS.DicomViewer.Domain.Anonymization
{
    public class PixelAnonymizationTemplate
    {
        public PixelAnonymizationTemplateId Id { get; private set; }
        public string TemplateName { get; private set; }
        public object Definition { get; private set; }

        private PixelAnonymizationTemplate() { }

        public PixelAnonymizationTemplate(PixelAnonymizationTemplateId id, string templateName, object definition)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            TemplateName = templateName ?? throw new ArgumentNullException(nameof(templateName));
            Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        }
    }
}