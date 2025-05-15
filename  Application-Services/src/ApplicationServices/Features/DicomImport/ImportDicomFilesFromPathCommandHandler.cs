using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using TheSSS.DicomViewer.Application.Interfaces.Infrastructure;
using TheSSS.DicomViewer.Application.Interfaces.Persistence;
using TheSSS.DicomViewer.Application.Interfaces.Application;
using TheSSS.DicomViewer.Application.DTOs.DicomImport;

namespace TheSSS.DicomViewer.Application.Features.DicomImport
{
    public class ImportDicomFilesFromPathCommandHandler : IRequestHandler<ImportDicomFilesFromPathCommand, DicomImportResultDto>
    {
        private readonly IFileSystemService _fileSystemService;
        private readonly IDicomValidationService _validationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IInstanceRepository _instanceRepository;
        private readonly IStudyRepository _studyRepository;
        private readonly ISeriesRepository _seriesRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IAuditLogRepository _auditLogRepository;
        private readonly ILogger<ImportDicomFilesFromPathCommandHandler> _logger;

        public ImportDicomFilesFromPathCommandHandler(
            IFileSystemService fileSystemService,
            IDicomValidationService validationService,
            IUnitOfWork unitOfWork,
            IInstanceRepository instanceRepository,
            IStudyRepository studyRepository,
            ISeriesRepository seriesRepository,
            IPatientRepository patientRepository,
            IAuditLogRepository auditLogRepository,
            ILogger<ImportDicomFilesFromPathCommandHandler> logger)
        {
            _fileSystemService = fileSystemService;
            _validationService = validationService;
            _unitOfWork = unitOfWork;
            _instanceRepository = instanceRepository;
            _studyRepository = studyRepository;
            _seriesRepository = seriesRepository;
            _patientRepository = patientRepository;
            _auditLogRepository = auditLogRepository;
            _logger = logger;
        }

        public async Task<DicomImportResultDto> Handle(ImportDicomFilesFromPathCommand request, CancellationToken cancellationToken)
        {
            var result = new DicomImportResultDto();
            
            try
            {
                await _unitOfWork.BeginTransactionAsync(cancellationToken);
                var files = await _fileSystemService.EnumerateFilesAsync(request.Path, "*.dcm");
                
                foreach (var file in files)
                {
                    var fileData = await _fileSystemService.ReadFileAsync(file);
                    var validationResult = await _validationService.ValidateDicomComplianceAsync(fileData);
                    
                    if (validationResult.IsValid)
                    {
                        // Metadata extraction and storage logic
                        // (Implementation details would depend on domain services)
                        result.FilesImportedCount++;
                    }
                    else
                    {
                        await _fileSystemService.MoveFileToQuarantineAsync(file);
                        result.FilesQuarantinedCount++;
                    }
                }
                
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing DICOM files from path {Path}", request.Path);
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                result.Success = false;
                result.Message = ex.Message;
            }
            
            return result;
        }
    }
}