using TheSSS.DICOMViewer.Monitoring.Contracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

/// <summary>
/// Adapter interface for accessing system error logs.
/// This abstracts access to critical system error logs from underlying logging services or application services.
/// </summary>
public interface ISystemErrorLogAdapter
{
    /// <summary>
    /// Retrieves a summary of critical system errors within a specified lookback period.
    /// </summary>
    /// <param name="lookbackPeriod">The duration to look back for critical errors.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains a <see cref="SystemErrorInfoSummaryDto"/> summarizing critical errors.
    /// </returns>
    Task<SystemErrorInfoSummaryDto> GetCriticalErrorSummaryAsync(TimeSpan lookbackPeriod, CancellationToken cancellationToken);
}