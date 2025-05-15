using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Application.Anonymization.DTOs
{
    public record AnonymizationResultDto(
        bool Success,
        string OriginalSopInstanceUid,
        string AnonymizedSopInstanceUid,
        List<string> Messages
    );
}