using TheSSS.DICOMViewer.Monitoring.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.Interfaces;

/// <summary>
/// Defines a contract for implementing alert throttling mechanisms to prevent alert fatigue.
/// </summary>
public interface IAlertThrottlingStrategy
{
    /// <summary>
    /// Determines if the given alert should be throttled based on the strategy's logic and configuration.
    /// </summary>
    /// <param name="alertContext">The context of the alert to be evaluated for throttling.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result is <c>true</c> if the alert should be throttled; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> ShouldThrottleAsync(AlertContextDto alertContext, CancellationToken cancellationToken);
}