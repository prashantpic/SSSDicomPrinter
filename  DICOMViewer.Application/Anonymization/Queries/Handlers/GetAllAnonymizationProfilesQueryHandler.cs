using MediatR;
using TheSSS.DICOMViewer.Application.Anonymization.DTOs;
using TheSSS.DICOMViewer.Infrastructure.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Application.Anonymization.Queries.Handlers
{
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
}