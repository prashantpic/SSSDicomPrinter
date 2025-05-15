namespace TheSSS.DICOMViewer.Monitoring.Interfaces;

using TheSSS.DICOMViewer.Monitoring.Configuration;

/// <summary>
/// Interface for providing alert rule configurations.
/// </summary>
public interface IAlertRuleConfigProvider
{
    /// <summary>
    /// Asynchronously retrieves all configured alert rules.
    /// </summary>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, returning an enumerable collection of AlertRule objects.</returns>
    Task<IEnumerable<AlertRule>> GetAlertRulesAsync(CancellationToken cancellationToken);
}