using System.Collections.Generic;

namespace TheSSS.DicomViewer.Application.DTOs.DicomImport
{
    public record DicomImportResultDto(
        bool Success,
        int FilesProcessedCount,
        int FilesImportedCount,
        int FilesQuarantinedCount,
        int FilesRejectedCount,
        List<string> ImportedInstanceUids,
        List<DicomValidationFailureDto> ValidationFailures,
        List<string> QuarantinedFilePaths,
        string Message);
}