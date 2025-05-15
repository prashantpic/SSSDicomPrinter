namespace TheSSS.DICOMViewer.Monitoring.Interfaces;

public interface IAlertRuleConfigProvider
{
    Task<IEnumerable<AlertRule>> GetAlertRulesAsync(CancellationToken cancellationToken);
}