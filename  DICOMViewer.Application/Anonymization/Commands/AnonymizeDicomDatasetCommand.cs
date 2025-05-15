using MediatR;
using TheSSS.DICOMViewer.Application.Anonymization.DTOs;

namespace TheSSS.DICOMViewer.Application.Anonymization.Commands;

public record AnonymizeDicomDatasetCommand(
    string SopInstanceUid,
    string AnonymizationProfileId
) : IRequest<AnonymizationResultDto>;