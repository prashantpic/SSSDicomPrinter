using MediatR;
using TheSSS.DicomViewer.Application.Interfaces.Infrastructure;
using TheSSS.DicomViewer.Application.Interfaces.Persistence;
using TheSSS.DicomViewer.Application.DTOs.Pacs;

namespace TheSSS.DicomViewer.Application.Features.DicomNetwork.CStoreScu;

public class SendDicomInstancesToPacsCommandHandler : IRequestHandler<SendDicomInstancesToPacsCommand, CStoreScuResultDto>
{
    private readonly IPacsConfigurationRepository _pacsRepository;
    private readonly IInstanceRepository _instanceRepository;
    private readonly IDicomCommsService _dicomComms;
    private readonly IAuditLogRepository _auditLog;

    public SendDicomInstancesToPacsCommandHandler(
        IPacsConfigurationRepository pacsRepository,
        IInstanceRepository instanceRepository,
        IDicomCommsService dicomComms,
        IAuditLogRepository auditLog)
    {
        _pacsRepository = pacsRepository;
        _instanceRepository = instanceRepository;
        _dicomComms = dicomComms;
        _auditLog = auditLog;
    }

    public async Task<CStoreScuResultDto> Handle(SendDicomInstancesToPacsCommand request, CancellationToken cancellationToken)
    {
        var result = new CStoreScuResultDto();
        var pacsConfig = await _pacsRepository.GetByIdAsync(request.PacsNodeId);
        var instances = await _instanceRepository.GetByUidsAsync(request.SopInstanceUids);

        foreach (var instance in instances)
        {
            try
            {
                await _dicomComms.SendCStoreAsync(pacsConfig, instance.FilePath);
                result.InstanceSendStatuses.Add(instance.SopInstanceUid, true);
            }
            catch (Exception ex)
            {
                result.InstanceSendStatuses.Add(instance.SopInstanceUid, false);
                await LogFailedTransfer(instance, ex);
            }
        }

        return result;
    }

    private async Task LogFailedTransfer(Instance instance, Exception ex)
    {
        await _auditLog.AddAsync(new AuditLog
        {
            EventType = "CStoreFailure",
            Description = $"Failed to send instance {instance.SopInstanceUid}",
            Details = ex.Message
        });
    }
}