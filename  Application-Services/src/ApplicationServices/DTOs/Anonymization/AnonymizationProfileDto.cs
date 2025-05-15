using System.Collections.Generic;

namespace TheSSS.DicomViewer.Application.DTOs.Anonymization
{
    public record AnonymizationProfileDto(
        int Id,
        string Name,
        string Description,
        Dictionary<string, string> TagRules,
        List<int> PixelTemplateIds);
}