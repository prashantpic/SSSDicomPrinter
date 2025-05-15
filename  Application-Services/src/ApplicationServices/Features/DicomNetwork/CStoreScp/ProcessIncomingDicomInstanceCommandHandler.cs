using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using TheSSS.DicomViewer.Application.Interfaces.Infrastructure;

namespace TheSSS.DicomViewer.Application.Features.DicomNetwork.CStoreScp
{
    public class ProcessIncomingDicomInstanceCommandHandler : IRequestHandler<ProcessIncomingDicomInstanceCommand, DicomImportResultDto>
    {
        private readonly IFileSystemService _fileSystemService;
        private readonly IDicomValidationService _validationService;
        private readonly ILogger<ProcessIncomingDicomInstanceCommandHandler> _logger;

        public ProcessIncomingDicomInstanceCommandHandler(
            IFileSystemService fileSystemService,
            IDicomValidationService validationService,
            ILogger<ProcessIncomingDicomInstanceCommandHandler> logger)
        {
            _fileSystemService = fileSystemService;
            _validationService = validationService;
            _logger = logger;
        }

        public async Task<DicomImportResultDto> Handle(ProcessIncomingDicomInstanceCommand request, CancellationToken cancellationToken)
        {
            var result = new DicomImportResultDto();
            
            try
            {
                var fileData = await _fileSystemService.ReadFileAsync(request.TempFilePath);
                var validationResult = await _validationService.ValidateDicomComplianceAsync(fileData);
                
                if (validationResult.IsValid)
                {
                    await _fileSystemService.StoreFileHierarchicallyAsync(request.TempFilePath);
                    result.FilesImportedCount++;
                }
                else
                {
                    await _fileSystemService.MoveFileToRejectedAsync(request.TempFilePath);
                    result.FilesRejectedCount++;
                }
                
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing incoming DICOM instance");
                result.Success = false;
                result.Message = ex.Message;
            }
            
            return result;
        }
    }
}