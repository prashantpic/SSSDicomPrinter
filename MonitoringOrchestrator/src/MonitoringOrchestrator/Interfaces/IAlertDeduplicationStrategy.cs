using TheSSS.DICOMViewer.Monitoring.Contracts;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.Interfaces;

/// <summary>
/// Defines a contract for implementing alert deduplication mechanisms to avoid redundant alerts.
/// </summary>
public interface IAlertDeduplicationStrategy
{
    /// <summary>
    /// Determines if the given alert is a duplicate of a recently processed alert based on the strategy's logic.
    /// </summary>
    /// <param name="alertContext">The context of the alert to be evaluated for duplication.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result is <c>true</c> if the alert is considered a duplicate; otherwise, <c>false</c>.
    /// </returns>
    Task<bool> IsDuplicateAsync(AlertContextDto alertContext, CancellationToken cancellationToken);
}