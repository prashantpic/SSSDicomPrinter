using MediatR;
using TheSSS.DICOMViewer.Application.Anonymization.DTOs;
using TheSSS.DICOMViewer.Infrastructure.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Application.Anonymization.Queries.Handlers
{
    public class GetAnonymizationProfileQueryHandler : IRequestHandler<GetAnonymizationProfileQuery, AnonymizationProfileDto>
    {
        private readonly ISettingsRepositoryAdapter _settingsRepository;

        public GetAnonymizationProfileQueryHandler(ISettingsRepositoryAdapter settingsRepository)
        {
            _settingsRepository = settingsRepository;
        }

        public async Task<AnonymizationProfileDto> Handle(GetAnonymizationProfileQuery request, CancellationToken cancellationToken)
        {
            var profile = await _settingsRepository.GetAnonymizationProfileByIdAsync(request.ProfileId)
                ?? throw new NotFoundException($"Profile with ID {request.ProfileId} not found");
            
            return profile;
        }
    }
}