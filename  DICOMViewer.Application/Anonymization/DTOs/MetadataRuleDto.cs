namespace TheSSS.DICOMViewer.Application.Anonymization.DTOs
{
    public record MetadataRuleDto(
        string DicomTagId,
        string Action,
        string ReplacementValue,
        string DateOffsetParameters
    );
}