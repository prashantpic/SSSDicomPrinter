using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace TheSSS.DICOMViewer.Monitoring.Mappers
{
    public static class HealthReportMapper
    {
        private static readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

        public static NotificationPayloadDto ToNotificationPayload(
            AlertContextDto alertContext,
            string channelType,
            List<string>? recipientDetails,
            string defaultSourceComponent)
        {
            var title = $"System Alert: {alertContext.Severity} - {alertContext.TriggeredRuleName}";
            var body = $"Severity: {alertContext.Severity}\n" +
                       $"Timestamp: {alertContext.Timestamp:yyyy-MM-dd HH:mm:ss UTC}\n" +
                       $"Source Component: {alertContext.SourceComponent ?? defaultSourceComponent}\n" +
                       $"Rule Name: {alertContext.TriggeredRuleName}\n" +
                       $"Message: {alertContext.Message}\n" +
                       $"Details:\n{FormatRawData(alertContext.RawData)}";

            return new NotificationPayloadDto
            {
                Title = title,
                Body = body,
                Severity = alertContext.Severity,
                Timestamp = alertContext.Timestamp,
                TargetChannelType = channelType,
                RecipientDetails = recipientDetails,
                SourceComponent = alertContext.SourceComponent ?? defaultSourceComponent,
                TriggeredRuleName = alertContext.TriggeredRuleName
            };
        }

        private static string FormatRawData(object? rawData)
        {
            if (rawData == null) return "N/A";

            try
            {
                return JsonSerializer.Serialize(rawData, _jsonSerializerOptions);
            }
            catch (Exception)
            {
                // Fallback if serialization fails (e.g., complex object graph)
                return rawData.ToString() ?? "Raw data could not be serialized.";
            }
        }

        public static OverallHealthStatus DetermineOverallStatus(HealthReportDto report)
        {
            if (report == null) return OverallHealthStatus.Unknown;

            var statuses = new List<OverallHealthStatus>();

            if (report.DatabaseHealth != null)
            {
                if (!report.DatabaseHealth.IsConnected) statuses.Add(OverallHealthStatus.Error);
            }

            if (report.LicenseStatus != null)
            {
                if (!report.LicenseStatus.IsValid && (report.LicenseStatus.DaysUntilExpiry == null || report.LicenseStatus.DaysUntilExpiry <= 0))
                    statuses.Add(OverallHealthStatus.Error);
                else if (report.LicenseStatus.DaysUntilExpiry.HasValue && report.LicenseStatus.DaysUntilExpiry.Value <= 30 && report.LicenseStatus.DaysUntilExpiry.Value > 0) // Assuming 30 days warning
                    statuses.Add(OverallHealthStatus.Warning);
            }

            if (report.SystemErrorSummary != null)
            {
                if (report.SystemErrorSummary.CriticalErrorCountLast24Hours > 0) // Assuming any critical error is a CRITICAL overall state
                    statuses.Add(OverallHealthStatus.Error); // Or Critical, depending on desired mapping. Let's map to Error.
            }

            if (report.StorageHealth != null)
            {
                // Example threshold, should ideally come from config or rule evaluation
                if (report.StorageHealth.UsedPercentage > 95) statuses.Add(OverallHealthStatus.Error);
                else if (report.StorageHealth.UsedPercentage > 85) statuses.Add(OverallHealthStatus.Warning);
            }

            if (report.PacsConnections != null && report.PacsConnections.Any(p => !p.IsConnected))
            {
                // If all PACS are down, it might be an Error. If some, Warning.
                if (report.PacsConnections.All(p => !p.IsConnected && report.PacsConnections.Any())) statuses.Add(OverallHealthStatus.Error);
                else statuses.Add(OverallHealthStatus.Warning);
            }
            
            if (report.AutomatedTaskStatuses != null)
            {
                foreach(var task in report.AutomatedTaskStatuses)
                {
                    if (task.LastRunStatus.Equals("Failed", StringComparison.OrdinalIgnoreCase))
                    {
                        statuses.Add(OverallHealthStatus.Warning); // A single failed task could be a warning or error
                        break; 
                    }
                }
            }

            if (statuses.Contains(OverallHealthStatus.Error)) return OverallHealthStatus.Error;
            if (statuses.Contains(OverallHealthStatus.Critical)) return OverallHealthStatus.Critical; // If Critical enum is used distinctly
            if (statuses.Contains(OverallHealthStatus.Warning)) return OverallHealthStatus.Warning;

            return OverallHealthStatus.Healthy;
        }

        public static AlertContextDto CreateAlertContext(
            AlertRule rule,
            object triggeringData,
            string message,
            string? sourceComponent = null)
        {
            AlertSeverity severity;
            if (!Enum.TryParse<AlertSeverity>(rule.Severity, true, out severity))
            {
                severity = AlertSeverity.Warning; // Default if parsing fails
            }

            return new AlertContextDto
            {
                TriggeredRuleName = rule.RuleName,
                Severity = severity,
                Timestamp = DateTime.UtcNow,
                SourceComponent = sourceComponent ?? "MonitoringOrchestrator",
                Message = message,
                RawData = triggeringData,
                AlertInstanceId = Guid.NewGuid()
            };
        }
    }
}