using TheSSS.DICOMViewer.Monitoring.Configuration;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.Interfaces;

/// <summary>
/// Defines a contract for providing alert rule configurations.
/// This allows abstraction over how alert rules are loaded (e.g., from configuration files, database).
/// </summary>
public interface IAlertRuleConfigProvider
{
    /// <summary>
    /// Asynchronously retrieves all configured alert rules.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>
    /// A task that represents the asynchronous operation. 
    /// The task result contains an enumerable collection of <see cref="AlertRule"/> instances.
    /// </returns>
    Task<IEnumerable<AlertRule>> GetAlertRulesAsync(CancellationToken cancellationToken);
}