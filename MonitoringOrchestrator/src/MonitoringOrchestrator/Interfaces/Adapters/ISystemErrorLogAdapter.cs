namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

using TheSSS.DICOMViewer.Monitoring.Contracts;

/// <summary>
/// Adapter interface for accessing system error logs.
/// Implementation is expected to be provided by REPO-CROSS-CUTTING (e.g., LoggingService)
/// or REPO-APP-SERVICES, capable of querying persisted logs.
/// </summary>
public interface ISystemErrorLogAdapter
{
    /// <summary>
    /// Retrieves a summary of critical system errors within a specified lookback period.
    /// </summary>
    /// <param name="lookbackPeriod">The time span to look back for errors (e.g., last 24 hours).</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains
    /// a SystemErrorInfoSummaryDto summarizing critical errors.
    /// </returns>
    Task<SystemErrorInfoSummaryDto> GetCriticalErrorSummaryAsync(TimeSpan lookbackPeriod, CancellationToken cancellationToken);
}