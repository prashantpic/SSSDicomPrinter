using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Application.DicomImport.DTOs;

public record ImportResultDto
{
    public int SuccessfullyImportedCount { get; init; }
    public int FailedOrQuarantinedCount { get; init; }
    public List<string> ProcessedFileNames { get; init; } = new();
    public List<string> ErrorMessagesOrQuarantineReasons { get; init; } = new();
}