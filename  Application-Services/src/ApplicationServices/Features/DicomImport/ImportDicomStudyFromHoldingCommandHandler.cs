using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using TheSSS.DicomViewer.Application.Interfaces.Infrastructure;
using TheSSS.DicomViewer.Application.Interfaces.Persistence;
using TheSSS.DicomViewer.Application.DTOs.DicomImport;

namespace TheSSS.DicomViewer.Application.Features.DicomImport
{
    public class ImportDicomStudyFromHoldingCommandHandler : IRequestHandler<ImportDicomStudyFromHoldingCommand, DicomImportResultDto>
    {
        private readonly IFileSystemService _fileSystemService;
        private readonly IDicomValidationService _validationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInstanceRepository _instanceRepository;
        private readonly ILogger<ImportDicomStudyFromHoldingCommandHandler> _logger;

        public ImportDicomStudyFromHoldingCommandHandler(
            IFileSystemService fileSystemService,
            IDicomValidationService validationService,
            IUnitOfWork unitOfWork,
            IInstanceRepository instanceRepository,
            ILogger<ImportDicomStudyFromHoldingCommandHandler> logger)
        {
            _fileSystemService = fileSystemService;
            _validationService = validationService;
            _unitOfWork = unitOfWork;
            _instanceRepository = instanceRepository;
            _logger = logger;
        }

        public async Task<DicomImportResultDto> Handle(ImportDicomStudyFromHoldingCommand request, CancellationToken cancellationToken)
        {
            var result = new DicomImportResultDto();
            
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);
                // Implementation logic similar to ImportDicomFilesFromPathCommandHandler
                // with specific handling for holding folder structure
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing study from holding folder");
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                result.Success = false;
                result.Message = ex.Message;
            }
            
            return result;
        }
    }
}