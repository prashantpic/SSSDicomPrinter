using MediatR;
using TheSSS.DicomViewer.Application.DTOs.DicomImport;

namespace TheSSS.DicomViewer.Application.Features.DicomImport
{
    public record ImportDicomStudyFromHoldingCommand(string StudyInstanceUidOrPath) 
        : IRequest<DicomImportResultDto>;
}