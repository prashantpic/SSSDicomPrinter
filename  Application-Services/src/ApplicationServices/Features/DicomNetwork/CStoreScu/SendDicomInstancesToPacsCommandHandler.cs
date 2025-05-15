using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using TheSSS.DicomViewer.Application.Interfaces.Persistence;
using TheSSS.DicomViewer.Application.Interfaces.Infrastructure;
using TheSSS.DicomViewer.Application.DTOs.Pacs;

namespace TheSSS.DicomViewer.Application.Features.DicomNetwork.CStoreScu
{
    public class SendDicomInstancesToPacsCommandHandler : IRequestHandler<SendDicomInstancesToPacsCommand, CStoreScuResultDto>
    {
        private readonly IPacsConfigurationRepository _pacsConfigRepository;
        private readonly IInstanceRepository _instanceRepository;
        private readonly IDicomCommsService _dicomCommsService;
        private readonly ILogger<SendDicomInstancesToPacsCommandHandler> _logger;

        public SendDicomInstancesToPacsCommandHandler(
            IPacsConfigurationRepository pacsConfigRepository,
            IInstanceRepository instanceRepository,
            IDicomCommsService dicomCommsService,
            ILogger<SendDicomInstancesToPacsCommandHandler> logger)
        {
            _pacsConfigRepository = pacsConfigRepository;
            _instanceRepository = instanceRepository;
            _dicomCommsService = dicomCommsService;
            _logger = logger;
        }

        public async Task<CStoreScuResultDto> Handle(SendDicomInstancesToPacsCommand request, CancellationToken cancellationToken)
        {
            var result = new CStoreScuResultDto();
            
            try
            {
                var pacsConfig = await _pacsConfigRepository.GetByIdAsync(request.PacsNodeId);
                var instances = await _instanceRepository.GetByUidsAsync(request.SopInstanceUids);
                
                foreach (var instance in instances)
                {
                    var storeResult = await _dicomCommsService.SendCStoreAsync(pacsConfig, instance.FilePath);
                    if (storeResult.Success)
                        result.InstancesSent++;
                    else
                        result.FailedInstanceUids.Add(instance.SopInstanceUid);
                }
                
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending instances to PACS node {NodeId}", request.PacsNodeId);
                result.Success = false;
                result.Message = ex.Message;
            }
            
            return result;
        }
    }
}