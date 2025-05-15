using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Integration.Interfaces;

/// <summary>
/// Defines a contract for acquiring a permit to proceed with an operation, potentially blocking or delaying 
/// if the rate limit for a given resource key has been exceeded. This interface is for API rate limiting logic, 
/// enabling services to acquire permits before making external calls.
/// </summary>
public interface IRateLimiter
{
    /// <summary>
    /// Attempts to acquire a permit to proceed with an operation for a given resource.
    /// </summary>
    /// <param name="resourceKey">A key identifying the resource (e.g., "OdooApi", "DicomNetwork").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Task that completes when a permit is acquired, potentially after waiting.</returns>
    Task AcquirePermitAsync(string resourceKey, CancellationToken cancellationToken = default);
}