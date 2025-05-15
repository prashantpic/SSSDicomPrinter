using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Monitoring.Contracts;

namespace TheSSS.DICOMViewer.Monitoring.Interfaces;

/// <summary>
/// Interface for alert deduplication strategies.
/// Implementations decide if an alert is a duplicate of a recently processed one.
/// </summary>
public interface IAlertDeduplicationStrategy
{
    /// <summary>
    /// Determines if the given alert is a duplicate of a recently processed alert.
    /// Implementations should track recent alerts (e.g., by signature) and compare the current one.
    /// </summary>
    /// <param name="alertContext">The context of the alert being considered for dispatch.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Task resolving to true if the alert is considered a duplicate, false otherwise.</returns>
    Task<bool> IsDuplicateAsync(AlertContextDto alertContext, CancellationToken cancellationToken);
}