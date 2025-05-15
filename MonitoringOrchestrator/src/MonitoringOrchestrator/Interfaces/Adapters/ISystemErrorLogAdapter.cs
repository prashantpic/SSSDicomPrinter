using System;
using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Monitoring.Contracts;

namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

/// <summary>
/// Adapter interface for accessing system error logs (likely from REPO-CROSS-CUTTING LoggingService or REPO-APP-SERVICES).
/// </summary>
public interface ISystemErrorLogAdapter
{
    /// <summary>
    /// Retrieves a summary of critical system errors within a specified lookback period.
    /// "Critical" errors are typically those logged with Error or Fatal severity.
    /// </summary>
    /// <param name="lookbackPeriod">The time span to look back for critical errors.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Task resolving to the SystemErrorInfoSummaryDto.
    /// Should return a summary with zero counts if no critical errors are found.
    /// </returns>
    /// <exception cref="TheSSS.DICOMViewer.Monitoring.Exceptions.DataSourceUnavailableException">
    /// Thrown if the adapter cannot access the error logs (e.g., log service down, query failed).
    /// </exception>
    Task<SystemErrorInfoSummaryDto> GetCriticalErrorSummaryAsync(TimeSpan lookbackPeriod, CancellationToken cancellationToken);
}