using Microsoft.Extensions.Options;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

namespace TheSSS.DICOMViewer.Monitoring.UseCaseHandlers;

public class AlertEvaluationService
{
    private readonly ILoggerAdapter<AlertEvaluationService> _logger;
    private readonly AlertingOptions _alertingOptions;
    private readonly IAlertRuleConfigProvider _ruleConfigProvider;
    private readonly AlertDispatchService _alertDispatchService;

    // In-memory state for consecutive failures (RuleName -> InstanceIdentifier -> Count)
    // This is a simple in-memory approach. For persistence across restarts, a database or distributed cache would be needed.
    private static readonly ConcurrentDictionary<string, ConcurrentDictionary<string, int>> _consecutiveFailureCounts = new();


    public AlertEvaluationService(
        ILoggerAdapter<AlertEvaluationService> logger,
        IOptions<AlertingOptions> alertingOptions,
        IAlertRuleConfigProvider ruleConfigProvider,
        AlertDispatchService alertDispatchService)
    {
        _logger = logger;
        _alertingOptions = alertingOptions?.Value ?? throw new ArgumentNullException(nameof(alertingOptions));
        _ruleConfigProvider = ruleConfigProvider ?? throw new ArgumentNullException(nameof(ruleConfigProvider));
        _alertDispatchService = alertDispatchService ?? throw new ArgumentNullException(nameof(alertDispatchService));
    }

    public async Task EvaluateHealthReportAsync(HealthReportDto healthReport, CancellationToken cancellationToken)
    {
        _logger.Info("Starting alert evaluation based on health report.");

        if (!_alertingOptions.IsEnabled)
        {
            _logger.Info("Alerting is disabled in global configuration. Skipping evaluation.");
            return;
        }

        var configuredRules = await _ruleConfigProvider.GetAlertRulesAsync(cancellationToken);
        if (configuredRules == null || !configuredRules.Any())
        {
            _logger.Warning("No alert rules configured or provider returned null/empty. Skipping evaluation.");
            return;
        }

        foreach (var rule in configuredRules)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                _logger.Info("Alert evaluation cancelled.");
                return;
            }

            _logger.Debug($"Evaluating rule: {rule.RuleName} (Metric: {rule.MetricType})");
            bool ruleTriggered = false;
            string triggerMessage = "";
            object? relevantData = null;
            string instanceKey = rule.InstanceIdentifier ?? "DefaultInstance"; // For stateful checks

            switch (rule.MetricType)
            {
                case "OverallStatus": // Example: Alert if overall status is Error
                    if (healthReport.OverallStatus == rule.ThresholdValue.ToString() /* Assuming ThresholdValue stores target string like "Error" */ )
                    {
                        ruleTriggered = EvaluateCondition(healthReport.OverallStatus, rule.ComparisonOperator, rule.ThresholdValue.ToString()!); // Compare strings
                        if(ruleTriggered)
                        {
                            triggerMessage = $"Overall system status is '{healthReport.OverallStatus}'.";
                            relevantData = new { healthReport.OverallStatus, healthReport.Timestamp };
                        }
                    }
                    break;

                case "StorageUsagePercent":
                    if (healthReport.StorageInfo != null && EvaluateCondition(healthReport.StorageInfo.UsedPercentage, rule.ComparisonOperator, rule.ThresholdValue))
                    {
                        ruleTriggered = true;
                        triggerMessage = $"Storage usage at '{healthReport.StorageInfo.StorageIdentifier}' is {healthReport.StorageInfo.UsedPercentage:F1}%, rule condition met (Operator: {rule.ComparisonOperator}, Threshold: {rule.ThresholdValue}%).";
                        relevantData = healthReport.StorageInfo;
                    }
                    break;

                case "DatabaseConnectivity":
                    if (healthReport.DatabaseConnectivity != null)
                    {
                        bool isConnected = healthReport.DatabaseConnectivity.IsConnected;
                        // Rule might check for IsConnected == false (ThresholdValue = 0 for false, 1 for true with 'EqualTo')
                        // Or Latency > ThresholdValue
                        if (rule.ComparisonOperator.Contains("Latency")) // Custom way to denote latency check
                        {
                             if(EvaluateCondition(healthReport.DatabaseConnectivity.LatencyMs ?? -1, rule.ComparisonOperator.Replace("Latency",""), rule.ThresholdValue))
                             {
                                ruleTriggered = true;
                                triggerMessage = $"Database latency is {healthReport.DatabaseConnectivity.LatencyMs}ms, rule condition met (Operator: {rule.ComparisonOperator}, Threshold: {rule.ThresholdValue}ms).";
                                relevantData = healthReport.DatabaseConnectivity;
                             }
                        }
                        else if (EvaluateCondition(isConnected ? 1 : 0, rule.ComparisonOperator, rule.ThresholdValue)) // 0 for false, 1 for true
                        {
                            ruleTriggered = HandleStatefulCheck(rule, instanceKey, !isConnected, ref triggerMessage, $"Database is {(isConnected ? "connected" : "disconnected")}. Error: {healthReport.DatabaseConnectivity.ErrorMessage}", out relevantData);
                            if(ruleTriggered) relevantData = healthReport.DatabaseConnectivity; else relevantData = null;
                        }
                        else // Condition not met, reset consecutive failures for this rule/instance
                        {
                             ResetConsecutiveFailures(rule.RuleName, instanceKey);
                        }
                    }
                    break;

                case "PacsConnectivity":
                    if (healthReport.PacsConnections != null)
                    {
                        var pacsNodes = healthReport.PacsConnections;
                        if (!string.IsNullOrEmpty(rule.InstanceIdentifier)) // Rule for a specific PACS node
                        {
                            var specificNode = pacsNodes.FirstOrDefault(p => p.PacsNodeId == rule.InstanceIdentifier);
                            if (specificNode != null)
                            {
                                bool isConnected = specificNode.IsConnected;
                                // Assuming ThresholdValue = 0 means "not connected" for "EqualTo" operator
                                if (EvaluateCondition(isConnected ? 1:0, rule.ComparisonOperator, rule.ThresholdValue))
                                {
                                    ruleTriggered = HandleStatefulCheck(rule, specificNode.PacsNodeId, !isConnected, ref triggerMessage, $"PACS node '{specificNode.PacsNodeId}' is {(isConnected ? "connected" : "disconnected")}. Error: {specificNode.LastEchoErrorMessage}", out relevantData);
                                    if(ruleTriggered) relevantData = specificNode; else relevantData = null;
                                }
                                else
                                {
                                    ResetConsecutiveFailures(rule.RuleName, specificNode.PacsNodeId);
                                }
                            }
                        }
                        else // Rule for aggregate PACS status (e.g., count of disconnected nodes)
                        {
                            int disconnectedCount = pacsNodes.Count(p => !p.IsConnected);
                            if (EvaluateCondition(disconnectedCount, rule.ComparisonOperator, rule.ThresholdValue))
                            {
                                ruleTriggered = true; // Aggregate rules usually don't need stateful consecutive checks unless defined that way
                                triggerMessage = $"{disconnectedCount} PACS nodes are disconnected, rule condition met (Operator: {rule.ComparisonOperator}, Threshold: {rule.ThresholdValue}).";
                                relevantData = pacsNodes.Where(p => !p.IsConnected).ToList();
                            }
                        }
                    }
                    break;

                case "LicenseStatus":
                    if (healthReport.LicenseStatus != null)
                    {
                        if (!healthReport.LicenseStatus.IsValid && rule.ThresholdValue == 0 && rule.ComparisonOperator == "EqualTo" /* Invalid is value 0 */)
                        {
                            ruleTriggered = true;
                            triggerMessage = $"Application license is invalid. Status: {healthReport.LicenseStatus.StatusMessage}.";
                            relevantData = healthReport.LicenseStatus;
                        }
                        else if (healthReport.LicenseStatus.IsValid && healthReport.LicenseStatus.DaysUntilExpiry.HasValue)
                        {
                            if (EvaluateCondition(healthReport.LicenseStatus.DaysUntilExpiry.Value, rule.ComparisonOperator, rule.ThresholdValue))
                            {
                                ruleTriggered = true;
                                triggerMessage = $"Application license expires in {healthReport.LicenseStatus.DaysUntilExpiry.Value} days, rule condition met (Operator: {rule.ComparisonOperator}, Threshold: {rule.ThresholdValue} days).";
                                relevantData = healthReport.LicenseStatus;
                            }
                        }
                    }
                    break;

                case "CriticalErrorCount":
                    if (healthReport.SystemErrorSummary != null && EvaluateCondition(healthReport.SystemErrorSummary.CriticalErrorCountLast24Hours, rule.ComparisonOperator, rule.ThresholdValue))
                    {
                        ruleTriggered = true;
                        triggerMessage = $"System critical error count in last 24 hours is {healthReport.SystemErrorSummary.CriticalErrorCountLast24Hours}, rule condition met (Operator: {rule.ComparisonOperator}, Threshold: {rule.ThresholdValue}).";
                        relevantData = healthReport.SystemErrorSummary;
                    }
                    break;
                
                case "AutomatedTaskFailureCount":
                     if (healthReport.AutomatedTaskStatuses != null)
                    {
                        var failedTasksCount = healthReport.AutomatedTaskStatuses.Count(t => t.LastRunStatus == "Failed");
                        if (EvaluateCondition(failedTasksCount, rule.ComparisonOperator, rule.ThresholdValue))
                        {
                            ruleTriggered = true;
                            triggerMessage = $"Found {failedTasksCount} automated tasks with 'Failed' status, rule condition met (Operator: {rule.ComparisonOperator}, Threshold: {rule.ThresholdValue}).";
                            relevantData = healthReport.AutomatedTaskStatuses.Where(t => t.LastRunStatus == "Failed").ToList();
                        }
                    }
                    break;

                default:
                    _logger.Warning($"Unsupported metric type '{rule.MetricType}' in rule '{rule.RuleName}'. Skipping.");
                    break;
            }

            if (ruleTriggered && relevantData != null)
            {
                _logger.Warning($"Alert rule triggered: {rule.RuleName}. Message: {triggerMessage}");
                var alertContext = new AlertContextDto
                {
                    TriggeredRuleName = rule.RuleName,
                    AlertSeverity = rule.Severity,
                    Timestamp = DateTimeOffset.UtcNow,
                    SourceComponent = rule.InstanceIdentifier ?? rule.MetricType, // Be more specific with source
                    Message = triggerMessage,
                    RawData = relevantData
                };
                await _alertDispatchService.DispatchAlertAsync(alertContext, cancellationToken);
            }
             else if (!ruleTriggered && rule.ConsecutiveFailuresToAlert > 1) // Reset if condition not met for stateful rules
            {
                // This part is a bit tricky. If the condition that *would* increment failure is false, reset.
                // The HandleStatefulCheck already does this for its specific path.
                // If the rule was not stateful or specific condition for failure increment wasn't met, this is fine.
            }
        }
        _logger.Info("Alert evaluation cycle completed.");
    }

    private bool HandleStatefulCheck(AlertRule rule, string instanceKey, bool isFailureConditionMet, ref string triggerMessage, string baseMessage, out object? relevantData)
    {
        relevantData = null;
        if (rule.ConsecutiveFailuresToAlert <= 1) // Not a stateful check beyond immediate
        {
            if (isFailureConditionMet)
            {
                triggerMessage = baseMessage;
                return true;
            }
            return false;
        }

        var ruleFailures = _consecutiveFailureCounts.GetOrAdd(rule.RuleName, _ => new ConcurrentDictionary<string, int>());
        
        if (isFailureConditionMet)
        {
            int currentFailures = ruleFailures.AddOrUpdate(instanceKey, 1, (_, count) => count + 1);
            _logger.Debug($"Stateful check for rule '{rule.RuleName}', instance '{instanceKey}': Failure condition met. Consecutive failures: {currentFailures}/{rule.ConsecutiveFailuresToAlert}.");
            if (currentFailures >= rule.ConsecutiveFailuresToAlert)
            {
                triggerMessage = $"{baseMessage} (Consecutive failures: {currentFailures} >= {rule.ConsecutiveFailuresToAlert})";
                // Optionally reset count after triggering to avoid immediate re-trigger unless desired
                // ruleFailures.TryUpdate(instanceKey, 0, currentFailures); 
                return true;
            }
            triggerMessage = $"{baseMessage} (Consecutive failures: {currentFailures}/{rule.ConsecutiveFailuresToAlert}, threshold not met)";
            return false;
        }
        else // Failure condition NOT met, reset consecutive failures for this rule/instance
        {
            ResetConsecutiveFailures(rule.RuleName, instanceKey);
            _logger.Debug($"Stateful check for rule '{rule.RuleName}', instance '{instanceKey}': Failure condition NOT met. Resetting consecutive failures.");
            return false;
        }
    }

    private void ResetConsecutiveFailures(string ruleName, string instanceKey)
    {
        if (_consecutiveFailureCounts.TryGetValue(ruleName, out var instanceFailures))
        {
            if (instanceFailures.TryRemove(instanceKey, out int Rval))
            {
                 _logger.Debug($"Reset consecutive failures for rule '{ruleName}', instance '{instanceKey}'. Previous count: {Rval}.");
            }
        }
    }

    private bool EvaluateCondition<T>(T value, string comparisonOperator, T thresholdValue) where T : IComparable
    {
        try
        {
            int comparisonResult = value.CompareTo(thresholdValue);
            return comparisonOperator switch
            {
                "GreaterThan" => comparisonResult > 0,
                "GreaterThanOrEqualTo" => comparisonResult >= 0,
                "LessThan" => comparisonResult < 0,
                "LessThanOrEqualTo" => comparisonResult <= 0,
                "EqualTo" => comparisonResult == 0,
                "NotEqualTo" => comparisonResult != 0,
                _ => {
                    _logger.Warning($"Unsupported comparison operator: {comparisonOperator} for type {typeof(T).Name}.");
                    return false;
                }
            };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"Error during typed condition evaluation for value {value} and threshold {thresholdValue} with operator {comparisonOperator}.");
            return false;
        }
    }
    
    private bool EvaluateCondition(double value, string comparisonOperator, double thresholdValue)
    {
        return EvaluateCondition<double>(value, comparisonOperator, thresholdValue);
    }
}