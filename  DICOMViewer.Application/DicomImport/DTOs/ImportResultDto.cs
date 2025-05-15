using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Application.DicomImport.DTOs
{
    public record ImportResultDto(
        int SuccessfullyImportedCount,
        int FailedOrQuarantinedCount,
        List<string> ProcessedFileNames,
        List<string> ErrorMessagesOrQuarantineReasons
    );
}