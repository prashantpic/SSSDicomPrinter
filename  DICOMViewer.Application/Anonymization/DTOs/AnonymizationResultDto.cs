using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Application.Anonymization.DTOs;

public record AnonymizationResultDto
{
    public bool Success { get; init; }
    public string OriginalSopInstanceUid { get; init; } = default!;
    public string? AnonymizedSopInstanceUid { get; init; }
    public List<string> Messages { get; init; } = new();
}