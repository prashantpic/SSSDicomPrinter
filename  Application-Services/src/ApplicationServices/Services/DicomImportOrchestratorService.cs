using MediatR;
using TheSSS.DicomViewer.Application.Features.DicomImport.Commands;
using TheSSS.DicomViewer.Application.Interfaces.Application;

namespace TheSSS.DicomViewer.Application.Services;

public class DicomImportOrchestratorService : IDicomImportService
{
    private readonly IMediator _mediator;

    public DicomImportOrchestratorService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<DicomImportResultDto> ImportFromPathAsync(string path, CancellationToken cancellationToken = default)
    {
        var command = new ImportDicomFilesFromPathCommand(path);
        return await _mediator.Send(command, cancellationToken);
    }

    public async Task<DicomImportResultDto> ImportFromHoldingFolderAsync(string studyInstanceUidOrPath, CancellationToken cancellationToken = default)
    {
        var command = new ImportDicomStudyFromHoldingCommand(studyInstanceUidOrPath);
        return await _mediator.Send(command, cancellationToken);
    }
}