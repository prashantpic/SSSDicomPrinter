using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using TheSSS.DicomViewer.Application.Interfaces.Persistence;
using TheSSS.DicomViewer.Application.Interfaces.Infrastructure;
using TheSSS.DicomViewer.Application.DTOs.Pacs;

namespace TheSSS.DicomViewer.Application.Features.DicomNetwork.CEchoScu
{
    public class VerifyPacsConnectionCommandHandler : IRequestHandler<VerifyPacsConnectionCommand, CEchoResultDto>
    {
        private readonly IPacsConfigurationRepository _pacsConfigRepository;
        private readonly IDicomCommsService _dicomCommsService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<VerifyPacsConnectionCommandHandler> _logger;

        public VerifyPacsConnectionCommandHandler(
            IPacsConfigurationRepository pacsConfigRepository,
            IDicomCommsService dicomCommsService,
            IUnitOfWork unitOfWork,
            ILogger<VerifyPacsConnectionCommandHandler> logger)
        {
            _pacsConfigRepository = pacsConfigRepository;
            _dicomCommsService = dicomCommsService;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CEchoResultDto> Handle(VerifyPacsConnectionCommand request, CancellationToken cancellationToken)
        {
            var result = new CEchoResultDto();
            
            try
            {
                var pacsConfig = await _pacsConfigRepository.GetByIdAsync(request.PacsNodeId);
                var echoResult = await _dicomCommsService.SendCEchoAsync(pacsConfig);
                
                result.Success = echoResult.IsSuccess;
                result.Message = echoResult.Message;
                result.ResponseTime = echoResult.Duration;
                
                // Update PACS status in database
                await _pacsConfigRepository.UpdateAsync(pacsConfig);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying PACS connection for node {NodeId}", request.PacsNodeId);
                result.Success = false;
                result.Message = ex.Message;
            }
            
            return result;
        }
    }
}