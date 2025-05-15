using System.Collections.Generic;

namespace TheSSS.DicomViewer.Application.DTOs.Anonymization
{
    public record AnonymizationResultDto(
        bool IsSuccessful,
        string SourceSopInstanceUid,
        string AnonymizedSopInstanceUid,
        string ErrorMessage,
        List<string> ModifiedTags);
}