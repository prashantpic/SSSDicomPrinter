namespace TheSSS.DICOMViewer.Monitoring.Interfaces;

using TheSSS.DICOMViewer.Monitoring.Contracts;

/// <summary>
/// Interface for alert throttling strategies.
/// </summary>
public interface IAlertThrottlingStrategy
{
    /// <summary>
    /// Determines if the given alert should be throttled based on the strategy's logic.
    /// The strategy should also update its internal state if the alert is not throttled,
    /// to record that this alert instance (or one like it) has been processed.
    /// </summary>
    /// <param name="alertContext">The context of the alert to evaluate.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, returning true if the alert should be throttled, false otherwise.</returns>
    Task<bool> ShouldThrottleAsync(AlertContextDto alertContext, CancellationToken cancellationToken);
}