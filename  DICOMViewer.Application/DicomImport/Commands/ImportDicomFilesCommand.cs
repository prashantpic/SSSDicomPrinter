using MediatR;
using System.Collections.Generic;
using TheSSS.DICOMViewer.Application.DicomImport.DTOs;

namespace TheSSS.DICOMViewer.Application.DicomImport.Commands;

public record ImportDicomFilesCommand : IRequest<ImportResultDto>
{
    public IEnumerable<string>? FilePaths { get; init; }
    public string? DirectoryPath { get; init; }
    public bool IsDirectoryImport { get; init; }
}