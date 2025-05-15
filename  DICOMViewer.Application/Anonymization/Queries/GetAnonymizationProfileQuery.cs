using MediatR;
using TheSSS.DICOMViewer.Application.Anonymization.DTOs;

namespace TheSSS.DICOMViewer.Application.Anonymization.Queries;

public record GetAnonymizationProfileQuery(
    string ProfileId
) : IRequest<AnonymizationProfileDto>;