namespace TheSSS.DicomViewer.Application.DTOs.DicomImport
{
    public record DicomValidationFailureDto(
        string FilePath,
        string FailureReason,
        string DicomTag = null,
        string Severity = "Error");
}