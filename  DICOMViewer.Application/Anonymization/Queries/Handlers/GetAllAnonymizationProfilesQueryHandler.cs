using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Application.Anonymization.Queries;
using TheSSS.DICOMViewer.Application.Interfaces.Infrastructure;

namespace TheSSS.DICOMViewer.Application.Anonymization.Queries.Handlers;

public class GetAllAnonymizationProfilesQueryHandler : IRequestHandler<GetAllAnonymizationProfilesQuery, IEnumerable<AnonymizationProfileDto>>
{
    private readonly ISettingsRepositoryAdapter _settingsRepository;

    public GetAllAnonymizationProfilesQueryHandler(ISettingsRepositoryAdapter settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }

    public async Task<IEnumerable<AnonymizationProfileDto>> Handle(GetAllAnonymizationProfilesQuery request, CancellationToken cancellationToken)
    {
        return await _settingsRepository.GetAllAnonymizationProfilesAsync();
    }
}