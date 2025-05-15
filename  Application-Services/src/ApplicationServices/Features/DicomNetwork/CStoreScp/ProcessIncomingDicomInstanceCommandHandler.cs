using MediatR;
using TheSSS.DicomViewer.Application.Interfaces.Infrastructure;
using TheSSS.DicomViewer.Application.Interfaces.Persistence;

namespace TheSSS.DicomViewer.Application.Features.DicomNetwork.CStoreScp;

public class ProcessIncomingDicomInstanceCommandHandler : IRequestHandler<ProcessIncomingDicomInstanceCommand, DicomValidationFailureDto>
{
    private readonly IFileSystemService _fileSystem;
    private readonly IDicomValidationService _validator;
    private readonly IConfigurationService _config;
    private readonly IAuditLogRepository _auditLog;

    public ProcessIncomingDicomInstanceCommandHandler(
        IFileSystemService fileSystem,
        IDicomValidationService validator,
        IConfigurationService config,
        IAuditLogRepository auditLog)
    {
        _fileSystem = fileSystem;
        _validator = validator;
        _config = config;
        _auditLog = auditLog;
    }

    public async Task<DicomValidationFailureDto> Handle(ProcessIncomingDicomInstanceCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateDicomFileAsync(request.TempFilePath, cancellationToken);
        if (!validationResult.IsValid)
        {
            await HandleInvalidFile(request.TempFilePath, validationResult);
            return new DicomValidationFailureDto(request.TempFilePath, 
                validationResult.ErrorCode, 
                validationResult.ErrorMessage);
        }

        var holdingPath = _config.GetValue<string>("HoldingFolderPath");
        await _fileSystem.MoveFileAsync(request.TempFilePath, holdingPath);
        return null;
    }

    private async Task HandleInvalidFile(string filePath, DicomValidationResult result)
    {
        var rejectPath = _config.GetValue<string>("RejectedArchivePath");
        await _fileSystem.MoveFileAsync(filePath, rejectPath);
        await _auditLog.AddAsync(new AuditLog
        {
            EventType = "InvalidDicomReceived",
            Description = $"Rejected invalid DICOM file: {Path.GetFileName(filePath)}",
            Details = result.ErrorMessage
        });
    }
}