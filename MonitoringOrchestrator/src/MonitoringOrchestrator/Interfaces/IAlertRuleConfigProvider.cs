using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Monitoring.Configuration;

namespace TheSSS.DICOMViewer.Monitoring.Interfaces;

/// <summary>
/// Interface for providing alert rule configurations.
/// This allows dynamic loading of rules if needed, otherwise can just wrap IOptions<AlertingOptions>.
/// </summary>
public interface IAlertRuleConfigProvider
{
    /// <summary>
    /// Asynchronously retrieves all configured alert rules.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A Task resolving to an enumerable collection of AlertRule definitions.
    /// Returns an empty collection if no rules are configured or available.
    /// </returns>
    Task<IEnumerable<AlertRule>> GetAlertRulesAsync(CancellationToken cancellationToken);
}