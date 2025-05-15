using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.UseCaseHandlers;

public class AlertEvaluationService
{
    private readonly AlertingOptions _options;
    private readonly AlertDispatchService _dispatchService;
    private readonly ILogger<AlertEvaluationService> _logger;
    private readonly Dictionary<string, int> _failureCounts = new();

    public AlertEvaluationService(
        IOptions<AlertingOptions> options,
        AlertDispatchService dispatchService,
        ILogger<AlertEvaluationService> logger)
    {
        _options = options.Value;
        _dispatchService = dispatchService;
        _logger = logger;
    }

    public async Task EvaluateHealthReportAsync(HealthReportDto report)
    {
        var alerts = new List<AlertContextDto>();
        
        foreach (var rule in _options.Rules)
        {
            var key = $"{rule.MetricType}-{rule.SubMetricIdentifier}";
            var shouldAlert = EvaluateRule(rule, report, key);

            if (shouldAlert)
            {
                alerts.Add(CreateAlertContext(rule, report));
                ResetFailureCount(key);
            }
            else
            {
                IncrementFailureCount(key);
            }
        }

        foreach (var alert in alerts)
        {
            await _dispatchService.DispatchAlertAsync(alert);
        }
    }

    private bool EvaluateRule(AlertRule rule, HealthReportDto report, string key)
    {
        return rule.MetricType switch
        {
            "StorageUsage" => report.StorageHealth?.UsedPercentage >= rule.ThresholdValue,
            "DatabaseConnectivity" => report.DatabaseHealth?.IsConnected == false,
            "PACSConnectivity" => report.PacsConnections?.Any(p => p.PacsNodeId == rule.SubMetricIdentifier && !p.IsConnected) == true,
            "LicenseStatus" => report.LicenseStatus?.IsValid == false,
            "SystemErrors" => report.SystemErrorSummary?.CriticalErrorCountLast24Hours >= rule.ThresholdValue,
            _ => false
        } && _failureCounts.GetValueOrDefault(key, 0) >= rule.ConsecutiveFailuresToAlert;
    }

    private AlertContextDto CreateAlertContext(AlertRule rule, HealthReportDto report)
    {
        return new AlertContextDto
        {
            TriggeredRuleName = rule.RuleName,
            Severity = Enum.Parse<AlertSeverity>(rule.Severity),
            SourceComponent = "MonitoringOrchestrator",
            Message = $"Alert triggered: {rule.RuleName}",
            RawData = GetRelevantData(rule, report)
        };
    }

    private object GetRelevantData(AlertRule rule, HealthReportDto report)
    {
        return rule.MetricType switch
        {
            "StorageUsage" => report.StorageHealth,
            "DatabaseConnectivity" => report.DatabaseHealth,
            "PACSConnectivity" => report.PacsConnections?.FirstOrDefault(p => p.PacsNodeId == rule.SubMetricIdentifier),
            "LicenseStatus" => report.LicenseStatus,
            "SystemErrors" => report.SystemErrorSummary,
            _ => null
        };
    }

    private void IncrementFailureCount(string key)
    {
        _failureCounts[key] = _failureCounts.GetValueOrDefault(key, 0) + 1;
    }

    private void ResetFailureCount(string key)
    {
        _failureCounts.Remove(key);
    }
}