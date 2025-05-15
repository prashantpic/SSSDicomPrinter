using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Application.Anonymization.DTOs;

public record AnonymizationProfileDto
{
    public string ProfileId { get; init; } = default!;
    public string ProfileName { get; init; } = default!;
    public string ProfileDescription { get; init; } = default!;
    public List<MetadataRuleDto> MetadataRules { get; init; } = new();
    public string? PredefinedRuleSetName { get; init; }
    public int? PixelAnonymizationTemplateId { get; init; }
    public bool IsReadOnly { get; init; }
}