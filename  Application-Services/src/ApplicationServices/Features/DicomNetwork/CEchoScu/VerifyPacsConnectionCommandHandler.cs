using MediatR;
using TheSSS.DicomViewer.Application.Interfaces.Infrastructure;
using TheSSS.DicomViewer.Application.Interfaces.Persistence;
using TheSSS.DicomViewer.Application.DTOs.Pacs;

namespace TheSSS.DicomViewer.Application.Features.DicomNetwork.CEchoScu;

public class VerifyPacsConnectionCommandHandler : IRequestHandler<VerifyPacsConnectionCommand, CEchoResultDto>
{
    private readonly IPacsConfigurationRepository _pacsRepository;
    private readonly IDicomCommsService _dicomComms;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogRepository _auditLog;
    private readonly IAlertingService _alerter;

    public VerifyPacsConnectionCommandHandler(
        IPacsConfigurationRepository pacsRepository,
        IDicomCommsService dicomComms,
        IUnitOfWork unitOfWork,
        IAuditLogRepository auditLog,
        IAlertingService alerter)
    {
        _pacsRepository = pacsRepository;
        _dicomComms = dicomComms;
        _unitOfWork = unitOfWork;
        _auditLog = auditLog;
        _alerter = alerter;
    }

    public async Task<CEchoResultDto> Handle(VerifyPacsConnectionCommand request, CancellationToken cancellationToken)
    {
        var config = await _pacsRepository.GetByIdAsync(request.PacsNodeId);
        var result = new CEchoResultDto();

        try
        {
            var response = await _dicomComms.SendCEchoAsync(config);
            result.IsSuccessful = response.Success;
            result.ResponseTime = response.Duration;
            
            config.LastSuccessfulConnection = DateTime.UtcNow;
            await _pacsRepository.UpdateAsync(config);
        }
        catch (Exception ex)
        {
            result.IsSuccessful = false;
            result.ErrorMessage = ex.Message;
            await HandleFailedConnection(config, ex);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return result;
    }

    private async Task HandleFailedConnection(PacsConfiguration config, Exception ex)
    {
        await _auditLog.AddAsync(new AuditLog
        {
            EventType = "PacsConnectionFailure",
            Description = $"Failed C-ECHO to {config.AeTitle}",
            Details = ex.Message
        });

        if (config.ConsecutiveFailures >= 3)
        {
            await _alerter.SendAlertAsync("PACS Connection Critical", 
                $"Multiple failures connecting to {config.AeTitle}");
        }
    }
}