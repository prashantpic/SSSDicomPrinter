using System.Collections.Generic;

namespace TheSSS.DicomViewer.Application.DTOs.Anonymization
{
    public record AnonymizationProfileDto(
        int Id,
        string Name,
        string Description,
        bool IsDefault,
        List<AnonymizationRuleDto> Rules,
        List<PixelAnonymizationTemplateDto> PixelTemplates);
}