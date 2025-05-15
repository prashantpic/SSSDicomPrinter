using MediatR;
using TheSSS.DicomViewer.Application.DTOs.DicomImport;

namespace TheSSS.DicomViewer.Application.Features.DicomNetwork.CStoreScp
{
    public record ProcessIncomingDicomInstanceCommand(
        string TempFilePath,
        string SourceAE,
        string CalledAE) : IRequest<DicomImportResultDto>;
}