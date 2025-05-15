using System;

namespace TheSSS.DicomViewer.Application.DTOs.Anonymization
{
    public record AnonymizationResultDto(
        bool Success,
        string Message,
        string SourceSopInstanceUid,
        string AnonymizedSopInstanceUid,
        int ProfileIdUsed,
        DateTime Timestamp);
}