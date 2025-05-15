using System.Collections.Generic;

namespace TheSSS.DicomViewer.Application.DTOs.Anonymization
{
    public record PixelAnonymizationTemplateDto(
        int Id,
        string Name,
        List<PixelRegionDto> Regions);
}