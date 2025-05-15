using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Integration.Interfaces;

/// <summary>
/// Defines a contract for acquiring a permit to proceed with an operation, 
/// potentially blocking or delaying if the rate limit for a given resource key has been exceeded.
/// </summary>
public interface IRateLimiter
{
    /// <summary>
    /// Acquires a permit for a specific resource, waiting if necessary until a permit is available or the operation is canceled.
    /// </summary>
    /// <param name="resourceKey">A key identifying the rate-limited resource or operation.</param>
    /// <param name="cancellationToken">A token to cancel the waiting operation.</param>
    /// <returns>A task representing the asynchronous permit acquisition.</returns>
    Task AcquirePermitAsync(string resourceKey, CancellationToken cancellationToken = default);
}