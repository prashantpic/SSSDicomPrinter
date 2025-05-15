using MediatR;
using System.Collections.Generic;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Application.DicomImport.Commands;
using TheSSS.DICOMViewer.Application.DicomImport.DTOs;
using TheSSS.DICOMViewer.Application.DicomImport.Interfaces;

namespace TheSSS.DICOMViewer.Application.DicomImport.Services;

public class DicomImportService : IDicomImportService
{
    private readonly IMediator _mediator;

    public DicomImportService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<ImportResultDto> ImportFilesAsync(IEnumerable<string> filePaths)
    {
        var command = new ImportDicomFilesCommand 
        { 
            FilePaths = filePaths,
            IsDirectoryImport = false
        };
        return await _mediator.Send(command);
    }

    public async Task<ImportResultDto> ImportDirectoryAsync(string directoryPath)
    {
        var command = new ImportDicomFilesCommand 
        { 
            DirectoryPath = directoryPath,
            IsDirectoryImport = true
        };
        return await _mediator.Send(command);
    }
}