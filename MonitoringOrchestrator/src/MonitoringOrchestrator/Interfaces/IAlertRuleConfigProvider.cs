namespace TheSSS.DICOMViewer.Monitoring.Interfaces;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Monitoring.Configuration;

public interface IAlertRuleConfigProvider
{
    Task<IEnumerable<AlertRule>> GetAlertRulesAsync(CancellationToken cancellationToken);
}