using MediatR;
using TheSSS.DicomViewer.Application.Interfaces.Infrastructure;
using TheSSS.DicomViewer.Application.Interfaces.Persistence;

namespace TheSSS.DicomViewer.Application.Features.DicomNetwork.CStoreScp;

public class RejectIncomingStudyCommandHandler : IRequestHandler<RejectIncomingStudyCommand>
{
    private readonly IFileSystemService _fileSystem;
    private readonly IAuditLogRepository _auditLog;
    private readonly IConfigurationService _config;

    public RejectIncomingStudyCommandHandler(
        IFileSystemService fileSystem,
        IAuditLogRepository auditLog,
        IConfigurationService config)
    {
        _fileSystem = fileSystem;
        _auditLog = auditLog;
        _config = config;
    }

    public async Task Handle(RejectIncomingStudyCommand request, CancellationToken cancellationToken)
    {
        var holdingPath = _config.GetValue