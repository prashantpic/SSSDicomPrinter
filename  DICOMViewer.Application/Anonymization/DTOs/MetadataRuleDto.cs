namespace TheSSS.DICOMViewer.Application.Anonymization.DTOs;

public record MetadataRuleDto
{
    public string DicomTagId { get; init; } = default!;
    public string Action { get; init; } = default!;
    public string? ReplacementValue { get; init; }
    public string? DateOffsetParameters { get; init; }
}