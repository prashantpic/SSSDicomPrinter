using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Persistence
{
    public interface IUserSettingsRepository
    {
        Task<UserSetting> GetByUserIdentifierAsync(string userIdentifier, CancellationToken cancellationToken);
        Task AddAsync(UserSetting userSetting, CancellationToken cancellationToken);
        Task UpdateAsync(UserSetting userSetting, CancellationToken cancellationToken);
        Task DeleteAsync(UserSetting userSetting, CancellationToken cancellationToken);
    }
}