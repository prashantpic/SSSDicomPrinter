namespace TheSSS.DicomViewer.Application.DTOs.DicomImport
{
    public record DicomValidationFailureDto(
        string FilePath,
        string ErrorCode,
        string ErrorMessage);
}