namespace TheSSS.DicomViewer.Application.DTOs.Anonymization
{
    public record PixelAnonymizationTemplateDto(
        int Id,
        string Name,
        string Description,
        string RegionType,
        string CoordinatesJson,
        string MaskType);
}