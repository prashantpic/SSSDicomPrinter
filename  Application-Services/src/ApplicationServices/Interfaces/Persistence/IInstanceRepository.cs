using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Application.Interfaces.Persistence
{
    public interface IInstanceRepository
    {
        Task<Instance> GetBySopInstanceUidAsync(string sopInstanceUid, CancellationToken cancellationToken);
        Task AddAsync(Instance instance, CancellationToken cancellationToken);
        Task UpdateAsync(Instance instance, CancellationToken cancellationToken);
        Task DeleteAsync(Instance instance, CancellationToken cancellationToken);
        Task<List<string>> GetFilePathsBySopInstanceUidsAsync(IEnumerable<string> sopInstanceUids, CancellationToken cancellationToken);
        Task UpdateStatusAsync(string sopInstanceUid, bool isAnonymized, CancellationToken cancellationToken);
    }
}