using MediatR;
using TheSSS.DicomViewer.Application.Interfaces.Infrastructure;
using TheSSS.DicomViewer.Application.Interfaces.Persistence;
using TheSSS.DicomViewer.Application.DTOs.DicomImport;
using TheSSS.DicomViewer.Domain.Models;
using Microsoft.Extensions.Logging;

namespace TheSSS.DicomViewer.Application.Features.DicomImport;

public class ImportDicomFilesFromPathCommandHandler : IRequestHandler<ImportDicomFilesFromPathCommand, DicomImportResultDto>
{
    private readonly IFileSystemService _fileSystemService;
    private readonly IDicomValidationService _dicomValidationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInstanceRepository _instanceRepository;
    private readonly IStudyRepository _studyRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IMapper _mapper;
    private readonly IConfigurationService _configService;
    private readonly ILogger<ImportDicomFilesFromPathCommandHandler> _logger;

    public ImportDicomFilesFromPathCommandHandler(
        IFileSystemService fileSystemService,
        IDicomValidationService dicomValidationService,
        IUnitOfWork unitOfWork,
        IInstanceRepository instanceRepository,
        IStudyRepository studyRepository,
        IPatientRepository patientRepository,
        IAuditLogRepository auditLogRepository,
        IMapper mapper,
        IConfigurationService configService,
        ILogger<ImportDicomFilesFromPathCommandHandler> logger)
    {
        _fileSystemService = fileSystemService;
        _dicomValidationService = dicomValidationService;
        _unitOfWork = unitOfWork;
        _instanceRepository = instanceRepository;
        _studyRepository = studyRepository;
        _patientRepository = patientRepository;
        _auditLogRepository = auditLogRepository;
        _mapper = mapper;
        _configService = configService;
        _logger = logger;
    }

    public async Task<DicomImportResultDto> Handle(ImportDicomFilesFromPathCommand request, CancellationToken cancellationToken)
    {
        var result = new DicomImportResultDto();
        var files = await _fileSystemService.ListFilesInDirectoryAsync(request.Path, true, cancellationToken);

        foreach (var filePath in files)
        {
            try
            {
                var validationResult = await _dicomValidationService.ValidateDicomFileAsync(filePath, cancellationToken);
                if (!validationResult.IsValid)
                {
                    await HandleInvalidFile(filePath, validationResult, result);
                    continue;
                }

                var hierarchicalPath = await _fileSystemService.DetermineHierarchicalPathAsync(filePath);
                await _fileSystemService.SaveFileAsync(filePath, hierarchicalPath, cancellationToken);

                var metadata = await ParseDicomMetadata(filePath);
                await UpdateDatabaseEntities(metadata, hierarchicalPath);

                result.ImportedSopInstanceUids.Add(metadata.SopInstanceUid);
                await LogAuditEvent("ImportSuccess", $"Imported DICOM file: {filePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error importing DICOM file: {FilePath}", filePath);
                await HandleImportError(filePath, ex.Message, result);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return result;
    }

    private async Task HandleInvalidFile(string filePath, DicomValidationResult validationResult, DicomImportResultDto result)
    {
        result.ValidationFailures.Add(new DicomValidationFailureDto(filePath, 
            validationResult.ErrorCode, 
            validationResult.ErrorMessage));

        if (_configService.GetValue<bool>("EnableQuarantine"))
        {
            var quarantinePath = _configService.GetValue<string>("QuarantinePath");
            await _fileSystemService.MoveFileAsync(filePath, quarantinePath);
            result.QuarantinedFilePaths.Add(filePath);
        }

        await LogAuditEvent("ImportFailure", $"Invalid DICOM file: {filePath}");
    }

    private async Task UpdateDatabaseEntities(DicomMetadata metadata, string storagePath)
    {
        var patient = _mapper.Map<Patient>(metadata);
        var study = _mapper.Map<Study>(metadata);
        var series = _mapper.Map<Series>(metadata);
        var instance = _mapper.Map<Instance>(metadata);

        instance.FilePath = storagePath;

        await _patientRepository.AddOrUpdateAsync(patient);
        await _studyRepository.AddOrUpdateAsync(study);
        await _seriesRepository.AddOrUpdateAsync(series);
        await _instanceRepository.AddOrUpdateAsync(instance);
    }

    private async Task LogAuditEvent(string eventType, string description)
    {
        await _auditLogRepository.AddAsync(new AuditLog
        {
            Timestamp = DateTime.UtcNow,
            EventType = eventType,
            Description = description
        });
    }

    private async Task HandleImportError(string filePath, string error, DicomImportResultDto result)
    {
        result.ValidationFailures.Add(new DicomValidationFailureDto(filePath, "IMPORT_ERROR", error));
        await LogAuditEvent("ImportError", $"Error importing file: {filePath} - {error}");
    }

    private async Task<DicomMetadata> ParseDicomMetadata(string filePath)
    {
        // Implementation would use DICOM library to read metadata
        return await Task.FromResult(new DicomMetadata()); // Simplified for example
    }
}