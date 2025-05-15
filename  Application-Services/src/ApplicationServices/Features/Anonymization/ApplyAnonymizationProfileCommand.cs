using MediatR;
using TheSSS.DicomViewer.Application.DTOs.Anonymization;

namespace TheSSS.DicomViewer.Application.Features.Anonymization
{
    public record ApplyAnonymizationProfileCommand(
        string SopInstanceUid,
        int ProfileId) : IRequest<AnonymizationResultDto>;
}