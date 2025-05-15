using TheSSS.DICOMViewer.Monitoring.Contracts;
using System;
using System.Text;

namespace TheSSS.DICOMViewer.Monitoring.Mappers
{
    /// <summary>
    /// Handles mapping between internal data models and DTOs for health reporting.
    /// </summary>
    public static class HealthReportMapper
    {
        /// <summary>
        /// Converts an <see cref="AlertContextDto"/> into a <see cref="NotificationPayloadDto"/>.
        /// Formats the message body based on the raw data type and triggered rule.
        /// </summary>
        /// <param name="alertContext">The alert context to convert.</param>
        /// <returns>A <see cref="NotificationPayloadDto"/> suitable for dispatching.</returns>
        public static NotificationPayloadDto ToNotificationPayload(AlertContextDto alertContext)
        {
            if (alertContext == null)
            {
                throw new ArgumentNullException(nameof(alertContext));
            }

            var title = $"Alert: {alertContext.TriggeredRuleName} - {alertContext.Severity}";
            var bodyBuilder = new StringBuilder();

            bodyBuilder.AppendLine($"An alert of severity '{alertContext.Severity}' has been triggered for '{alertContext.SourceComponent}'.");
            bodyBuilder.AppendLine($"Rule: {alertContext.TriggeredRuleName}");
            bodyBuilder.AppendLine($"Timestamp: {alertContext.Timestamp:yyyy-MM-dd HH:mm:sszzz}");
            bodyBuilder.AppendLine($"Message: {alertContext.Message}");

            if (alertContext.RawData != null)
            {
                bodyBuilder.AppendLine("\nDetails:");
                switch (alertContext.RawData)
                {
                    case StorageHealthInfoDto storageInfo:
                        bodyBuilder.AppendLine($"- Path: {storageInfo.Path}");
                        bodyBuilder.AppendLine($"- Used Space: {storageInfo.UsedPercentage:F2}%");
                        bodyBuilder.AppendLine($"- Free Space: {FormatBytes(storageInfo.FreeSpaceBytes)}");
                        bodyBuilder.AppendLine($"- Total Capacity: {FormatBytes(storageInfo.TotalCapacityBytes)}");
                        break;
                    case DatabaseConnectivityInfoDto dbInfo:
                        bodyBuilder.AppendLine($"- Connected: {dbInfo.IsConnected}");
                        if (!dbInfo.IsConnected)
                            bodyBuilder.AppendLine($"- Error: {dbInfo.ErrorMessage}");
                        if(dbInfo.LatencyMs.HasValue)
                            bodyBuilder.AppendLine($"- Latency: {dbInfo.LatencyMs} ms");
                        break;
                    case PacsConnectionInfoDto pacsInfo:
                        bodyBuilder.AppendLine($"- PACS Node ID: {pacsInfo.PacsNodeId}");
                        bodyBuilder.AppendLine($"- Connected: {pacsInfo.IsConnected}");
                        if (!pacsInfo.IsConnected)
                            bodyBuilder.AppendLine($"- Error: {pacsInfo.LastEchoErrorMessage}");
                        break;
                    case LicenseStatusInfoDto licenseInfo:
                        bodyBuilder.AppendLine($"- Valid: {licenseInfo.IsValid}");
                        bodyBuilder.AppendLine($"- Status: {licenseInfo.StatusMessage}");
                        if (licenseInfo.ExpiryDate.HasValue)
                            bodyBuilder.AppendLine($"- Expiry Date: {licenseInfo.ExpiryDate:yyyy-MM-dd}");
                        if (licenseInfo.DaysUntilExpiry.HasValue)
                            bodyBuilder.AppendLine($"- Days Until Expiry: {licenseInfo.DaysUntilExpiry}");
                        break;
                    case SystemErrorInfoSummaryDto errorSummary:
                        bodyBuilder.AppendLine($"- Critical Errors (24h): {errorSummary.CriticalErrorCountLast24Hours}");
                        if(errorSummary.ErrorTypeSummaries != null && errorSummary.ErrorTypeSummaries.Any())
                        {
                            bodyBuilder.AppendLine("- Top Error Types:");
                            foreach(var summary in errorSummary.ErrorTypeSummaries)
                            {
                                bodyBuilder.AppendLine($"  - Type: {summary.Type}, Count: {summary.Count}");
                            }
                        }
                        break;
                    case AutomatedTaskStatusInfoDto taskStatus:
                        bodyBuilder.AppendLine($"- Task Name: {taskStatus.TaskName}");
                        bodyBuilder.AppendLine($"- Last Run Status: {taskStatus.LastRunStatus}");
                        if (taskStatus.LastRunTimestamp.HasValue)
                            bodyBuilder.AppendLine($"- Last Run: {taskStatus.LastRunTimestamp:yyyy-MM-dd HH:mm:sszzz}");
                        if (!string.IsNullOrEmpty(taskStatus.ErrorMessage))
                            bodyBuilder.AppendLine($"- Error: {taskStatus.ErrorMessage}");
                        break;
                    default:
                        bodyBuilder.AppendLine($"- Raw Data: {alertContext.RawData.ToString()}");
                        break;
                }
            }

            return new NotificationPayloadDto
            {
                Title = title,
                Body = bodyBuilder.ToString(),
                Severity = alertContext.Severity,
                Timestamp = alertContext.Timestamp,
                SourceComponent = alertContext.SourceComponent
                // RecipientDetails will be handled by the specific channel if needed
            };
        }

        private static string FormatBytes(long bytes)
        {
            string[] suffix = { "B", "KB", "MB", "GB", "TB" };
            int i;
            double dblSByte = bytes;
            for (i = 0; i < suffix.Length && bytes >= 1024; i++, bytes /= 1024)
            {
                dblSByte = bytes / 1024.0;
            }
            return $"{dblSByte:0.##} {suffix[i]}";
        }
    }
}