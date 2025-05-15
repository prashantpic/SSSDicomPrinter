using MediatR;
using TheSSS.DICOMViewer.Application.DicomImport.Commands;
using TheSSS.DICOMViewer.Application.DicomImport.DTOs;

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
        var command = new ImportDicomFilesCommand { FilePaths = filePaths };
        return await _mediator.Send(command);
    }

    public async Task<ImportResultDto> ImportDirectoryAsync(string directoryPath)
    {
        var command = new ImportDicomFilesCommand { DirectoryPath = directoryPath };
        return await _mediator.Send(command);
    }
}