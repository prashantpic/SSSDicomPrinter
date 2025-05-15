using System.Collections.Generic;

namespace TheSSS.DicomViewer.Application.DTOs.DicomImport
{
    public record DicomImportResultDto(
        bool IsSuccess,
        List<string> ImportedSopInstanceUids,
        List<string> QuarantinedFilePaths,
        List<DicomValidationFailureDto> ValidationFailures);
}