using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Application.Anonymization.Queries;
using TheSSS.DICOMViewer.Application.Interfaces.Infrastructure;

namespace TheSSS.DICOMViewer.Application.Anonymization.Queries.Handlers;

public class GetAnonymizationProfileQueryHandler : IRequestHandler<GetAnonymizationProfileQuery, AnonymizationProfileDto>
{
    private readonly ISettingsRepositoryAdapter _settingsRepository;

    public GetAnonymizationProfileQueryHandler(ISettingsRepositoryAdapter settingsRepository)
    {
        _settingsRepository = settingsRepository;
    }

    public async Task<AnonymizationProfileDto> Handle(GetAnonymizationProfileQuery request, CancellationToken cancellationToken)
    {
        return await _settingsRepository.GetAnonymizationProfileByIdAsync(request.ProfileId);
    }
}