namespace TheSSS.DICOMViewer.Application.DicomNetwork.DTOs;

public record DicomNetworkResultDto
{
    public bool Success { get; init; }
    public string Message { get; init; } = default!;
    public string? Detail { get; init; }
}