using MediatR;
using TheSSS.DicomViewer.Application.Interfaces.Infrastructure;
using TheSSS.DicomViewer.Application.Interfaces.Persistence;
using TheSSS.DicomViewer.Application.DTOs.DicomImport;

namespace TheSSS.DicomViewer.Application.Features.DicomImport;

public class ImportDicomStudyFromHoldingCommandHandler : IRequestHandler<ImportDicomStudyFromHoldingCommand, DicomImportResultDto>
{
    private readonly IFileSystemService _fileSystemService;
    private readonly IDicomValidationService _dicomValidationService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInstanceRepository _instanceRepository;
    private readonly IStudyRepository _studyRepository;
    private readonly IPatientRepository _patientRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IConfigurationService _configService;

    public ImportDicomStudyFromHoldingCommandHandler(
        IFileSystemService fileSystemService,
        IDicomValidationService dicomValidationService,
        IUnitOfWork unitOfWork,
        IInstanceRepository instanceRepository,
        IStudyRepository studyRepository,
        IPatientRepository patientRepository,
        IAuditLogRepository auditLogRepository,
        IConfigurationService configService)
    {
        _fileSystemService = fileSystemService;
        _dicomValidationService = dicomValidationService;
        _unitOfWork = unitOfWork;
        _instanceRepository = instanceRepository;
        _studyRepository = studyRepository;
        _patientRepository = patientRepository;
        _auditLogRepository = auditLogRepository;
        _configService = configService;
    }

    public async Task<DicomImportResultDto> Handle(ImportDicomStudyFromHoldingCommand request, CancellationToken cancellationToken)
    {
        var result = new DicomImportResultDto();
        var holdingPath = _configService.GetValue<string>("HoldingFolderPath");
        var studyPath = Path.Combine(holdingPath, request.StudyInstanceUid);

        var files = await _fileSystemService.ListFilesInDirectoryAsync(studyPath, false, cancellationToken);

        foreach (var filePath in files)
        {
            // Similar processing to ImportDicomFilesFromPathCommandHandler
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        await _fileSystemService.DeleteDirectoryAsync(studyPath);
        return result;
    }
}