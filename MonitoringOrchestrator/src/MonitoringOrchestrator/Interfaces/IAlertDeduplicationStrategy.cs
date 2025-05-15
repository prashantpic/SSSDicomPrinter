namespace TheSSS.DICOMViewer.Monitoring.Interfaces;

using TheSSS.DICOMViewer.Monitoring.Contracts;

/// <summary>
/// Interface for alert deduplication strategies.
/// </summary>
public interface IAlertDeduplicationStrategy
{
    /// <summary>
    /// Determines if the given alert is a duplicate of a recently processed alert.
    /// </summary>
    /// <param name="alertContext">The context of the alert to evaluate.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, returning true if the alert is considered a duplicate, false otherwise.</returns>
    Task<bool> IsDuplicateAsync(AlertContextDto alertContext, CancellationToken cancellationToken);

    /// <summary>
    /// Registers an alert as having been processed, for future deduplication checks.
    /// This method should be called by the AlertDispatchService after an alert has been
    /// checked by IsDuplicateAsync and determined not to be a duplicate, and before
    /// (or immediately after) it's sent to channels.
    /// </summary>
    /// <param name="alertContext">The context of the processed alert.</param>
    void RegisterProcessedAlert(AlertContextDto alertContext);
}