using MediatR;
using System.Collections.Generic;
using TheSSS.DICOMViewer.Application.Anonymization.DTOs;

namespace TheSSS.DICOMViewer.Application.Anonymization.Queries;

public record GetAllAnonymizationProfilesQuery : IRequest<IEnumerable<AnonymizationProfileDto>>;