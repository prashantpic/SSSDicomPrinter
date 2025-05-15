```csharp
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Monitoring.UseCaseHandlers
{
    /// <summary>
    /// Service that evaluates unified health reports or specific health events 
    /// against configured alert rules to determine if an alert should be triggered.
    /// </summary>
    public class AlertEvaluationService
    {
        private readonly IAlertRuleConfigProvider _alertRuleConfigProvider;
        private readonly AlertDispatchService _alertDispatchService;
        private readonly ILogger<AlertEvaluationService> _logger;
        private readonly IOptions<AlertingOptions> _alertingOptions; // Can be used directly if IAlertRuleConfigProvider is simple

        // State for consecutive failures. Key: RuleName:TargetIdentifier (if any)
        private readonly ConcurrentDictionary<string, AlertFailureState> _consecutiveFailureStates = new();
        internal record AlertFailureState(int FailureCount, DateTime LastFailureTimestamp);


        /// <summary>
        /// Initializes a new instance of the <see cref="AlertEvaluationService"/> class.
        /// </summary>
        /// <param name="alertRuleConfigProvider">The provider for alert rule configurations.</param>
        /// <param name="alertDispatchService">The service responsible for dispatching alerts.</param>
        /// <param name="logger">The logger instance.</param>
        /// <param name="alertingOptions">The alerting configuration options.</param>
        public AlertEvaluationService(
            IAlertRuleConfigProvider alertRuleConfigProvider,
            AlertDispatchService alertDispatchService,
            ILogger<AlertEvaluationService> logger,
            IOptions<AlertingOptions> alertingOptions)
        {
            _alertRuleConfigProvider = alertRuleConfigProvider ?? throw new ArgumentNullException(nameof(alertRuleConfigProvider));
            _alertDispatchService = alertDispatchService ?? throw new ArgumentNullException(nameof(alertDispatchService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _alertingOptions = alertingOptions ?? throw new ArgumentNullException(nameof(alertingOptions));
        }

        /// <summary>
        /// Evaluates a health report against configured alert rules and triggers alerts if conditions are met.
        /// </summary>
        /// <param name="healthReport">The health report to evaluate.</param>
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task EvaluateHealthReportAsync(HealthReportDto healthReport, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting alert evaluation for health report timestamped {HealthReportTimestamp}.", healthReport.Timestamp);

            var rules = await _alertRuleConfigProvider.GetAlertRulesAsync(cancellationToken);
            if (rules == null || !rules.Any())
            {
                _logger.LogWarning("No alert rules configured. Skipping evaluation.");
                return;
            }

            var triggeredAlertContexts = new List<AlertContextDto>();

            // Evaluate Storage Info
            if (healthReport.StorageInfo != null)
            {
                EvaluateMetric(healthReport.StorageInfo, "StorageUsagePercent", healthReport.StorageInfo.UsedPercentage, healthReport.StorageInfo.Path, rules, triggeredAlertContexts);
                // Add more specific storage metric evaluations if needed
            }

            // Evaluate Database Connectivity
            if (healthReport.DatabaseConnectivity != null)
            {
                EvaluateMetric(healthReport.DatabaseConnectivity, "DatabaseIsConnected", healthReport.DatabaseConnectivity.IsConnected ? 1 : 0, null, rules, triggeredAlertContexts); // 1 for true, 0 for false
                if (healthReport.DatabaseConnectivity.LatencyMs.HasValue)
                {
                    EvaluateMetric(healthReport.DatabaseConnectivity, "DatabaseLatencyMs", healthReport.DatabaseConnectivity.LatencyMs.Value, null, rules, triggeredAlertContexts);
                }
            }

            // Evaluate PACS Statuses
            if (healthReport.PacsStatuses != null)
            {
                foreach (var pacsStatus in healthReport.PacsStatuses)
                {
                    EvaluateMetric(pacsStatus, "PacsConnectivity", pacsStatus.IsConnected ? 1 : 0, pacsStatus.PacsNodeId, rules, triggeredAlertContexts); // 1 for true, 0 for false
                }
            }

            // Evaluate License Status
            if (healthReport.LicenseStatus != null)
            {
                EvaluateMetric(healthReport.LicenseStatus, "LicenseIsValid", healthReport.LicenseStatus.IsValid ? 1 : 0, null, rules, triggeredAlertContexts); // 1 for true, 0 for false
                if (healthReport.LicenseStatus.DaysUntilExpiry.HasValue)
                {
                    EvaluateMetric(healthReport.LicenseStatus, "LicenseDaysUntilExpiry", healthReport.LicenseStatus.DaysUntilExpiry.Value, null, rules, triggeredAlertContexts);
                }
            }
            
            // Evaluate System Error Summary
            if (healthReport.SystemErrorSummary != null)
            {
                 EvaluateMetric(healthReport.SystemErrorSummary, "SystemCriticalErrorCountLast24Hours", healthReport.SystemErrorSummary.CriticalErrorCountLast24Hours, null, rules, triggeredAlertContexts);
            }

            // Evaluate Automated Task Statuses
            if (healthReport.AutomatedTaskStatuses != null)
            {
                foreach (var taskStatus in healthReport.AutomatedTaskStatuses)
                {
                    // Example: 1 if "Failed", 0 if "Success", potentially others
                    int statusValue = taskStatus.LastRunStatus?.Equals("Failed", StringComparison.OrdinalIgnoreCase) == true ? 1 : 0;
                    EvaluateMetric(taskStatus, "AutomatedTaskStatusFailed", statusValue, taskStatus.TaskName, rules, triggeredAlertContexts);
                }
            }


            foreach (var alertContext in triggeredAlertContexts)
            {
                await _alertDispatchService.DispatchAlertAsync(alertContext, cancellationToken);
            }

            _logger.LogInformation("Alert evaluation completed. {TriggeredCount} alerts processed.", triggeredAlertContexts.Count);
            CleanupOldFailureStates();
        }

        private void EvaluateMetric(object rawData, string metricType, double metricValue, string? targetIdentifier, IEnumerable<AlertRule> rules, List<AlertContextDto> triggeredAlertContexts)
        {
            foreach (var rule in rules.Where(r => r.MetricType.Equals(metricType, StringComparison.OrdinalIgnoreCase) &&
                                                  (string.IsNullOrEmpty(r.TargetIdentifier) || r.TargetIdentifier.Equals(targetIdentifier, StringComparison.OrdinalIgnoreCase))))
            {
                bool conditionMet = IsConditionMet(metricValue, rule.ThresholdValue, rule.ComparisonOperator);
                string stateKey = GenerateRuleStateKey(rule, targetIdentifier);

                if (conditionMet)
                {
                    var currentState = _consecutiveFailureStates.AddOrUpdate(
                        stateKey,
                        key => new AlertFailureState(1, DateTime.UtcNow),
                        (key, existingState) => new AlertFailureState(existingState.FailureCount + 1, DateTime.UtcNow)
                    );

                    if (currentState.FailureCount >= rule.ConsecutiveFailuresToAlert)
                    {
                        _logger.LogWarning("Alert rule '{RuleName}' triggered for MetricType '{MetricType}' (Target: {Target}) with value {MetricValue}. Consecutive failures: {FailureCount}",
                            rule.RuleName, rule.MetricType, targetIdentifier ?? "N/A", metricValue, currentState.FailureCount);

                        var alertContext = new AlertContextDto
                        {
                            TriggeredRuleName = rule.RuleName,
                            AlertSeverity = rule.Severity,
                            Timestamp = DateTimeOffset.UtcNow,
                            SourceComponent = DetermineSourceComponent(metricType, targetIdentifier),
                            Message = $"Rule '{rule.RuleName}' triggered: {metricType} " +
                                      $"{(targetIdentifier != null ? $"for {targetIdentifier} " : "")}" +
                                      $"is {metricValue}, which meets condition ({rule.ComparisonOperator} {rule.ThresholdValue}). " +
                                      $"Consecutive failures: {currentState.FailureCount}.",
                            RawData = rawData,
                            AlertHash = $"{rule.RuleName}-{metricType}-{targetIdentifier ?? "global"}-{rule.Severity}" // Basic hash
                        };
                        triggeredAlertContexts.Add(alertContext);
                    }
                }
                else // Condition not met, reset failure count if it was previously failing
                {
                    if (_consecutiveFailureStates.TryRemove(stateKey, out var removedState))
                    {
                         _logger.LogInformation("Condition for rule '{RuleName}' (Target: {Target}) no longer met. Resetting consecutive failure count from {FailureCount}.",
                            rule.RuleName, targetIdentifier ?? "N/A", removedState.FailureCount);
                    }
                }
            }
        }
        
        private string GenerateRuleStateKey(AlertRule rule, string? targetIdentifier)
        {
            return $"{rule.RuleName}_{rule.MetricType}_{targetIdentifier ?? "global"}";
        }

        private bool IsConditionMet(double actualValue, double thresholdValue, string comparisonOperator)
        {
            return comparisonOperator switch
            {
                "GreaterThan" => actualValue > thresholdValue,
                "GreaterThanOrEqualTo" => actualValue >= thresholdValue,
                "LessThan" => actualValue < thresholdValue,
                "LessThanOrEqualTo" => actualValue <= thresholdValue,
                "EqualTo" => Math.Abs(actualValue - thresholdValue) < 0.000001, // double comparison
                "NotEqualTo" => Math.Abs(actualValue - thresholdValue) >= 0.000001,
                _ => throw new ArgumentOutOfRangeException(nameof(comparisonOperator), $"Unsupported comparison operator: {comparisonOperator}")
            };
        }

        private string DetermineSourceComponent(string metricType, string? targetIdentifier)
        {
            // Basic determination, can be enhanced
            if (metricType.Contains("Storage")) return "Storage";
            if (metricType.Contains("Database")) return "Database";
            if (metricType.Contains("Pacs")) return targetIdentifier ?? "PACS";
            if (metricType.Contains("License")) return "License";
            if (metricType.Contains("Error")) return "SystemErrors";
            if (metricType.Contains("Task")) return targetIdentifier ?? "AutomatedTasks";
            return "System";
        }

        private void CleanupOldFailureStates()
        {
            // Clean up states for rules that haven't failed recently (e.g., older than 2x the longest check interval or a fixed duration)
            // This prevents unbounded growth of the dictionary if rules become healthy.
            // For simplicity, use a fixed duration, e.g., 1 day. This should be configurable or tied to monitoring intervals.
            var cutoff = DateTime.UtcNow.AddDays(-1); 
            foreach (var key in _consecutiveFailureStates.Keys)
            {
                if (_consecutiveFailureStates.TryGetValue(key, out var state) && state.LastFailureTimestamp < cutoff)
                {
                    _consecutiveFailureStates.TryRemove(key, out _);
                    _logger.LogDebug("Removed stale consecutive failure state for key: {StateKey}", key);
                }
            }
        }
    }
}
```