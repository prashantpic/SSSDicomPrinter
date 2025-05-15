namespace TheSSS.DICOMViewer.Application.Anonymization.DTOs;

public record PixelAnonymizationTemplateDto
{
    public int TemplateId { get; init; }
    public string TemplateName { get; init; } = default!;
    public string TemplateDefinitionJson { get; init; } = default!;
}