using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Persistence
{
    public interface IUserSettingsRepository
    {
        Task<Domain.Models.UserSetting> GetByUserIdentifierAsync(string userIdentifier, CancellationToken cancellationToken = default);
        Task AddOrUpdateAsync(Domain.Models.UserSetting userSetting, CancellationToken cancellationToken = default);
        Task DeleteByUserIdentifierAsync(string userIdentifier, CancellationToken cancellationToken = default);
    }
}