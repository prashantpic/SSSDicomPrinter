using MediatR;
using TheSSS.DICOMViewer.Application.Anonymization.DTOs;

namespace TheSSS.DICOMViewer.Application.Anonymization.Queries;

public record GetAllAnonymizationProfilesQuery : IRequest<IEnumerable<AnonymizationProfileDto>>;