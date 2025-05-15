namespace TheSSS.DICOMViewer.Application.DicomImport.DTOs
{
    public record ImportRequestDto(
        IEnumerable<string> FilePaths,
        string DirectoryPath
    );
}