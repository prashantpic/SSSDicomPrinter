namespace TheSSS.DICOMViewer.Application.DicomImport.DTOs;

public record ImportRequestDto
{
    public IEnumerable<string>? FilePaths { get; init; }
    public string? DirectoryPath { get; init; }
    public bool IsDirectoryImport { get; init; }
}