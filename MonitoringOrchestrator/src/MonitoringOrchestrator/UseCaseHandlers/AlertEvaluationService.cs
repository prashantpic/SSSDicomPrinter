using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.UseCaseHandlers;

public class AlertEvaluationService
{
    private readonly AlertingOptions _alertingOptions;
    private readonly AlertDispatchService _alertDispatchService;
    private readonly ILogger<AlertEvaluationService> _logger;
    private readonly Dictionary<string, int> _consecutiveFailures = new();

    public AlertEvaluationService(
        IOptions<AlertingOptions> alertingOptions,
        AlertDispatchService alertDispatchService,
        ILogger<AlertEvaluationService> logger)
    {
        _alertingOptions = alertingOptions.Value;
        _alertDispatchService = alertDispatchService;
        _logger = logger;
    }

    public async Task EvaluateHealthReportAsync(HealthReportDto healthReport, CancellationToken cancellationToken)
    {
        foreach (var rule in _alertingOptions.Rules)
        {
            var ruleKey = $"{rule.RuleName}-{rule.MetricType}";
            var conditionMet = EvaluateRule(rule, healthReport, out var triggeringData);

            if (conditionMet)
            {
                _consecutiveFailures[ruleKey] = _consecutiveFailures.TryGetValue(ruleKey, out var count) ? count + 1 : 1;
                
                if (_consecutiveFailures[ruleKey] >= rule.ConsecutiveFailuresToAlert)
                {
                    var context = CreateAlertContext(rule, triggeringData);
                    await _alertDispatchService.DispatchAlertAsync(context, cancellationToken);
                    _consecutiveFailures[ruleKey] = 0;
                }
            }
            else
            {
                _consecutiveFailures.Remove(ruleKey);
            }
        }
    }

    private bool EvaluateRule(AlertRule rule, HealthReportDto report, out object? triggeringData)
    {
        triggeringData = null;
        return rule.MetricType switch
        {
            "StorageUsagePercent" => CheckStorageRule(rule, report, ref triggeringData),
            "DatabaseConnectivity" => CheckDatabaseRule(rule, report, ref triggeringData),
            "PacsConnectivity" => CheckPacsRule(rule, report, ref triggeringData),
            "LicenseStatus" => CheckLicenseRule(rule, report, ref triggeringData),
            _ => false
        };
    }

    private bool CheckStorageRule(AlertRule rule, HealthReportDto report, ref object? data)
    {
        if (report.StorageHealth == null) return false;
        data = report.StorageHealth;
        return rule.ComparisonOperator switch
        {
            "GreaterThan" => report.StorageHealth.UsedPercentage > rule.ThresholdValue,
            _ => false
        };
    }

    private bool CheckDatabaseRule(AlertRule rule, HealthReportDto report, ref object? data)
    {
        if (report.DatabaseHealth == null) return false;
        data = report.DatabaseHealth;
        return rule.ExpectedStatus?.ToLower() == report.DatabaseHealth.IsConnected.ToString().ToLower();
    }

    private AlertContextDto CreateAlertContext(AlertRule rule, object? data)
    {
        return new AlertContextDto
        {
            TriggeredRuleName = rule.RuleName,
            Severity = Enum.Parse<AlertSeverity>(rule.Severity),
            Timestamp = DateTime.UtcNow,
            SourceComponent = "MonitoringOrchestrator",
            Message = $"Condition met for {rule.MetricType}",
            RawData = data
        };
    }
}