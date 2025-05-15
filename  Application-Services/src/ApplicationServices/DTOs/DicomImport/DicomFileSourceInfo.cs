namespace TheSSS.DicomViewer.Application.DTOs.DicomImport
{
    public record DicomFileSourceInfo(
        string FilePath,
        string SourceIdentifier,
        string StudyInstanceUid = null,
        string SeriesInstanceUid = null);
}