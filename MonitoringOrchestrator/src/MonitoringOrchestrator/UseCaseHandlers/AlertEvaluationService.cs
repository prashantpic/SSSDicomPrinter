namespace TheSSS.DICOMViewer.Monitoring.UseCaseHandlers;

using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using TheSSS.DICOMViewer.Monitoring.Mappers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class AlertEvaluationService
{
    private readonly AlertingOptions _alertingOptions;
    private readonly AlertDispatchService _alertDispatchService;
    private readonly ILogger<AlertEvaluationService> _logger;

    // In-memory state to track consecutive rule failures
    // Key: Unique rule identifier (RuleName + SubMetricIdentifier)
    // Value: Number of consecutive failures
    private readonly Dictionary<string, int> _consecutiveFailures = new Dictionary<string, int>();

    public AlertEvaluationService(
        IOptions<AlertingOptions> alertingOptions,
        AlertDispatchService alertDispatchService,
        ILogger<AlertEvaluationService> logger)
    {
        _alertingOptions = alertingOptions?.Value ?? throw new ArgumentNullException(nameof(alertingOptions));
        _alertDispatchService = alertDispatchService ?? throw new ArgumentNullException(nameof(alertDispatchService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Evaluates a HealthReportDto against configured alert rules.
    /// </summary>
    /// <param name="healthReport">The health report to evaluate.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task EvaluateHealthReportAsync(HealthReportDto healthReport, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Evaluating health report against alert rules.");

        var rules = _alertingOptions.Rules;
        if (rules == null || !rules.Any())
        {
            _logger.LogInformation("No alert rules configured. Skipping evaluation.");
            return;
        }

        var triggeredAlertContexts = new List<AlertContextDto>();

        foreach (var rule in rules)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.LogInformation("Alert evaluation cancelled.");
                break;
            }

            var ruleKey = GetRuleKey(rule);
            bool conditionMet = CheckRuleCondition(rule, healthReport, out object? triggeringData, out string? alertMessage);

            if (conditionMet)
            {
                _consecutiveFailures.TryGetValue(ruleKey, out int currentFailures);
                currentFailures++;
                _consecutiveFailures[ruleKey] = currentFailures;
                _logger.LogDebug($"Rule '{rule.RuleName}' (Key: {ruleKey}) condition met. Consecutive failures: {currentFailures}/{rule.ConsecutiveFailuresToAlert}.");

                if (currentFailures >= rule.ConsecutiveFailuresToAlert)
                {
                    _logger.LogWarning($"Rule '{rule.RuleName}' (Key: {ruleKey}) triggered after {currentFailures} consecutive checks.");
                    var alertContext = HealthReportMapper.CreateAlertContext(
                        rule,
                        triggeringData!,
                        alertMessage ?? $"Rule '{rule.RuleName}' condition met.",
                        _alertingOptions.DefaultAlertSourceComponent);
                    triggeredAlertContexts.Add(alertContext);
                    // Optionally reset consecutive failures for this rule instance immediately after triggering an alert,
                    // or wait until the condition is no longer met. Current logic resets when condition is false.
                }
            }
            else
            {
                if (_consecutiveFailures.ContainsKey(ruleKey) && _consecutiveFailures[ruleKey] > 0)
                {
                    _logger.LogInformation($"Rule '{rule.RuleName}' (Key: {ruleKey}) condition no longer met. Resetting consecutive failure count from {_consecutiveFailures[ruleKey]}.");
                    _consecutiveFailures[ruleKey] = 0; // Reset count
                }
            }
        }

        // Dispatch all triggered alerts
        foreach (var alertContext in triggeredAlertContexts)
        {
            if (cancellationToken.IsCancellationRequested) break;
            await _alertDispatchService.DispatchAlertAsync(alertContext, cancellationToken);
        }
        
        // Clean up entries with 0 failures to prevent memory leaks
        var keysToRemove = _consecutiveFailures.Where(pair => pair.Value == 0).Select(pair => pair.Key).ToList();
        foreach (var key in keysToRemove)
        {
            _consecutiveFailures.Remove(key);
        }

        _logger.LogDebug("Alert evaluation finished.");
    }

    private string GetRuleKey(AlertRule rule)
    {
        // A unique identifier for a rule instance, considering specific sub-metrics.
        return $"{rule.RuleName}_{rule.MetricType}_{rule.SubMetricIdentifier ?? "GLOBAL"}";
    }

    private bool CheckRuleCondition(AlertRule rule, HealthReportDto report, out object? triggeringData, out string? message)
    {
        triggeringData = null;
        message = null;

        try
        {
            object? dataSourceObject = null;
            string propertyName = rule.MetricType; // Simplified: MetricType maps directly to a property name or a part of it

            // Determine the primary data object from the report
            if (propertyName.StartsWith("Storage", StringComparison.OrdinalIgnoreCase)) { dataSourceObject = report.StorageHealth; propertyName = propertyName.Substring("Storage".Length); }
            else if (propertyName.StartsWith("Database", StringComparison.OrdinalIgnoreCase)) { dataSourceObject = report.DatabaseHealth; propertyName = propertyName.Substring("Database".Length); }
            else if (propertyName.StartsWith("Pacs", StringComparison.OrdinalIgnoreCase))
            {
                dataSourceObject = report.PacsConnections?.FirstOrDefault(p => p.PacsNodeId == rule.SubMetricIdentifier);
                propertyName = propertyName.Substring("Pacs".Length);
            }
            else if (propertyName.StartsWith("License", StringComparison.OrdinalIgnoreCase)) { dataSourceObject = report.LicenseStatus; propertyName = propertyName.Substring("License".Length); }
            else if (propertyName.StartsWith("SystemError", StringComparison.OrdinalIgnoreCase)) { dataSourceObject = report.SystemErrorSummary; propertyName = propertyName.Substring("SystemError".Length); }
            else if (propertyName.StartsWith("AutomatedTask", StringComparison.OrdinalIgnoreCase))
            {
                dataSourceObject = report.AutomatedTaskStatuses?.FirstOrDefault(t => t.TaskName == rule.SubMetricIdentifier);
                propertyName = propertyName.Substring("AutomatedTask".Length);
            }
            else if (propertyName.Equals("OverallHealthStatus", StringComparison.OrdinalIgnoreCase))
            {
                triggeringData = report; // The whole report for overall status
                message = $"Overall system health is {report.OverallStatus}. Expected {rule.ComparisonOperator} {rule.ExpectedStatus}.";
                return EvaluateStringCondition(report.OverallStatus.ToString(), rule.ComparisonOperator, rule.ExpectedStatus ?? "");
            }

            if (dataSourceObject == null)
            {
                _logger.LogTrace($"Data source object for MetricType '{rule.MetricType}' (Rule '{rule.RuleName}', SubMetric: '{rule.SubMetricIdentifier ?? "N/A"}') not found or null in health report.");
                return false;
            }
            triggeringData = dataSourceObject;

            // Get actual value using reflection (simplified, assumes MetricType maps to a property on dataSourceObject)
            // More robust implementation would use specific checks per MetricType
            var propertyInfo = dataSourceObject.GetType().GetProperty(propertyName, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            if (propertyInfo == null)
            {
                 // Try direct DTO property names if MetricType is more specific
                if (rule.MetricType == "StorageUsagePercent" && dataSourceObject is StorageHealthInfoDto sh) propertyInfo = sh.GetType().GetProperty(nameof(StorageHealthInfoDto.UsedPercentage));
                else if (rule.MetricType == "DatabaseConnectivity" && dataSourceObject is DatabaseConnectivityInfoDto dc) propertyInfo = dc.GetType().GetProperty(nameof(DatabaseConnectivityInfoDto.IsConnected));
                else if (rule.MetricType == "PacsConnectivity" && dataSourceObject is PacsConnectionInfoDto pc) propertyInfo = pc.GetType().GetProperty(nameof(PacsConnectionInfoDto.IsConnected));
                else if (rule.MetricType == "LicenseIsValid" && dataSourceObject is LicenseStatusInfoDto ls) propertyInfo = ls.GetType().GetProperty(nameof(LicenseStatusInfoDto.IsValid));
                else if (rule.MetricType == "LicenseDaysUntilExpiry" && dataSourceObject is LicenseStatusInfoDto ls_exp) propertyInfo = ls_exp.GetType().GetProperty(nameof(LicenseStatusInfoDto.DaysUntilExpiry));
                else if (rule.MetricType == "SystemErrorCount" && dataSourceObject is SystemErrorInfoSummaryDto ses) propertyInfo = ses.GetType().GetProperty(nameof(SystemErrorInfoSummaryDto.CriticalErrorCountLast24Hours));
                else if (rule.MetricType == "AutomatedTaskLastRunStatus" && dataSourceObject is AutomatedTaskStatusInfoDto ats) propertyInfo = ats.GetType().GetProperty(nameof(AutomatedTaskStatusInfoDto.LastRunStatus));
            }


            if (propertyInfo == null)
            {
                _logger.LogWarning($"Property '{propertyName}' (derived from MetricType '{rule.MetricType}') not found on '{dataSourceObject.GetType().Name}' for rule '{rule.RuleName}'.");
                return false;
            }

            object? actualValue = propertyInfo.GetValue(dataSourceObject);
            if (actualValue == null && rule.ThresholdValue != null) // Cannot compare null with a numeric threshold usually
            {
                 _logger.LogTrace($"Actual value for '{propertyName}' is null, cannot evaluate rule '{rule.RuleName}' against numeric threshold.");
                 return false;
            }


            message = $"Metric '{rule.MetricType}' (value: {actualValue ?? "null"}) {rule.ComparisonOperator} threshold/expected: '{rule.ThresholdValue?.ToString() ?? rule.ExpectedStatus ?? "N/A"}'.";

            // Evaluate condition
            if (actualValue is double || actualValue is int || actualValue is long || actualValue is float || actualValue is decimal)
            {
                if (!rule.ThresholdValue.HasValue)
                {
                    _logger.LogWarning($"Rule '{rule.RuleName}' expects numeric comparison but ThresholdValue is not set.");
                    return false;
                }
                return EvaluateNumericCondition(Convert.ToDouble(actualValue), rule.ComparisonOperator, rule.ThresholdValue.Value);
            }
            if (actualValue is bool booleanValue)
            {
                if (string.IsNullOrEmpty(rule.ExpectedStatus) || !bool.TryParse(rule.ExpectedStatus, out bool expectedBool))
                {
                     _logger.LogWarning($"Rule '{rule.RuleName}' expects boolean comparison but ExpectedStatus ('{rule.ExpectedStatus}') is not a valid boolean.");
                     return false;
                }
                return EvaluateBooleanCondition(booleanValue, rule.ComparisonOperator, expectedBool);
            }
            if (actualValue is string stringValue)
            {
                if (string.IsNullOrEmpty(rule.ExpectedStatus))
                {
                     _logger.LogWarning($"Rule '{rule.RuleName}' expects string comparison but ExpectedStatus is not set.");
                     return false;
                }
                return EvaluateStringCondition(stringValue, rule.ComparisonOperator, rule.ExpectedStatus);
            }
            if (actualValue is DateTime) // Potentially for timestamps if rules are defined for them
            {
                _logger.LogWarning($"DateTime comparison not yet implemented for rule '{rule.RuleName}'.");
                return false;
            }

            _logger.LogWarning($"Unsupported data type '{actualValue?.GetType().Name ?? "null"}' for metric '{rule.MetricType}' in rule '{rule.RuleName}'.");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error evaluating rule '{rule.RuleName}'.");
            return false;
        }
    }

    private bool EvaluateNumericCondition(double actual, string op, double threshold)
    {
        return op switch
        {
            "GreaterThan" => actual > threshold,
            "LessThan" => actual < threshold,
            "EqualTo" => actual == threshold, // Use tolerance for double comparison if exact match is risky
            "NotEqualTo" => actual != threshold,
            "GreaterThanOrEqualTo" => actual >= threshold,
            "LessThanOrEqualTo" => actual <= threshold,
            _ => { _logger.LogWarning($"Unsupported numeric comparison operator: {op}"); return false; }
        };
    }

    private bool EvaluateBooleanCondition(bool actual, string op, bool expected)
    {
        return op switch
        {
            "EqualTo" => actual == expected,
            "NotEqualTo" => actual != expected,
            _ => { _logger.LogWarning($"Unsupported boolean comparison operator: {op}"); return false; }
        };
    }

    private bool EvaluateStringCondition(string actual, string op, string expected)
    {
        if (actual == null) return false; // Cannot compare null string for equality/contains
        return op switch
        {
            "EqualTo" => actual.Equals(expected, StringComparison.OrdinalIgnoreCase),
            "NotEqualTo" => !actual.Equals(expected, StringComparison.OrdinalIgnoreCase),
            "Contains" => actual.Contains(expected, StringComparison.OrdinalIgnoreCase),
            "DoesNotContain" => !actual.Contains(expected, StringComparison.OrdinalIgnoreCase),
            // Add StartsWith, EndsWith if needed
            _ => { _logger.LogWarning($"Unsupported string comparison operator: {op}"); return false; }
        };
    }
}