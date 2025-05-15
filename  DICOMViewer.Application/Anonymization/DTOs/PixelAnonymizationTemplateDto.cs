namespace TheSSS.DICOMViewer.Application.Anonymization.DTOs
{
    public record PixelAnonymizationTemplateDto(
        int TemplateId,
        string TemplateName,
        string TemplateDefinitionJson
    );
}