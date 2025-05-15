using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Monitoring.Contracts;

namespace TheSSS.DICOMViewer.Monitoring.Interfaces;

/// <summary>
/// Interface for alert throttling strategies.
/// Implementations decide if an alert should be suppressed to prevent alert storms.
/// </summary>
public interface IAlertThrottlingStrategy
{
    /// <summary>
    /// Determines if the given alert should be throttled based on the strategy's logic.
    /// Implementations should track recent alerts and apply throttling rules.
    /// </summary>
    /// <param name="alertContext">The context of the alert being considered for dispatch.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Task resolving to true if the alert should be throttled (suppressed), false otherwise.</returns>
    Task<bool> ShouldThrottleAsync(AlertContextDto alertContext, CancellationToken cancellationToken);
}