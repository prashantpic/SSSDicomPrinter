namespace TheSSS.DICOMViewer.Application.Anonymization.DTOs;

public record AnonymizationRequestDto
{
    public string SopInstanceUid { get; init; } = default!;
    public string AnonymizationProfileId { get; init; } = default!;
}