using TheSSS.DICOMViewer.Monitoring.Contracts;

namespace TheSSS.DICOMViewer.Monitoring.Mappers;

/// <summary>
/// Handles mapping between internal data models and DTOs for health reporting if complex transformations are needed.
/// Currently focuses on mapping AlertContextDto to NotificationPayloadDto.
/// </summary>
public class HealthReportMapper // Make it instantiable for DI
{
    // Could inject IOptions<AlertingOptions> here if notification formatting depends on config

    public NotificationPayloadDto ToNotificationPayload(AlertContextDto alertContext)
    {
        // Basic mapping and formatting. More complex formatting could be implemented here
        // based on AlertContextDto.RawData or specific rules.
        var title = $"{alertContext.AlertSeverity.ToUpper()} Alert: {alertContext.TriggeredRuleName}";
        var body = $"Severity: {alertContext.AlertSeverity}\n" +
                   $"Source: {alertContext.SourceComponent}\n" +
                   $"Timestamp: {alertContext.Timestamp:yyyy-MM-dd HH:mm:ss}\n" +
                   $"Message: {alertContext.Message}\n\n";

        // Add details from RawData if needed (requires casting/checking type)
        if (alertContext.RawData != null)
        {
            body += "Details:\n";
            body += FormatRawDataDetails(alertContext.RawData);
        }

        return new NotificationPayloadDto
        {
            Title = title,
            Body = body,
            Severity = alertContext.AlertSeverity,
            Timestamp = alertContext.Timestamp,
            CorrelationId = alertContext.CorrelationId,
            // TargetChannelType and RecipientDetails will be set by AlertDispatchService
        };
    }

    private string FormatRawDataDetails(object rawData)
    {
        // Example formatting based on known DTO types
        return rawData switch
        {
            StorageHealthInfoDto storage => $"Storage Usage: {storage.UsedPercentage:F1}% ({FormatBytes(storage.FreeSpaceBytes)} free of {FormatBytes(storage.TotalCapacityBytes)})",
            DatabaseConnectivityInfoDto db => $"DB Connected: {db.IsConnected}, Latency: {db.LatencyMs}ms, Error: {db.ErrorMessage ?? "N/A"}",
            IEnumerable<PacsConnectionInfoDto> pacsList => "PACS Connections:\n" + string.Join("\n", pacsList.Select(p => $"  {p.PacsNodeId}: {(p.IsConnected ? "Connected" : "Disconnected")} (Error: {p.LastEchoErrorMessage ?? "N/A"})")),
            LicenseStatusInfoDto license => $"License Valid: {license.IsValid}, Status: {license.StatusMessage}, Expires: {license.ExpiryDate?.ToString("yyyy-MM-dd") ?? "N/A"} ({license.DaysUntilExpiry} days left)",
            SystemErrorInfoSummaryDto errors => $"Critical Errors (24h): {errors.CriticalErrorCountLast24Hours}\n" + string.Join("\n", errors.ErrorTypeSummaries.Select(e => $"  - {e.Type}: {e.Count}")),
             IEnumerable<AutomatedTaskStatusInfoDto> tasks => "Automated Task Statuses:\n" + string.Join("\n", tasks.Select(t => $"  {t.TaskName}: {t.LastRunStatus} (Last Run: {t.LastRunTimestamp?.ToString("yyyy-MM-dd HH:mm")}, Error: {t.ErrorMessage ?? "N/A"})")),
            // Add more cases for other DTO types
            _ => rawData.ToString() ?? "N/A" // Fallback to ToString()
        };
    }

    private string FormatBytes(long bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
        int suffixIndex = 0;
        double doubleBytes = bytes;

        while (doubleBytes >= 1024 && suffixIndex < suffixes.Length - 1)
        {
            doubleBytes /= 1024;
            suffixIndex++;
        }

        return $"{doubleBytes:F1}{suffixes[suffixIndex]}";
    }
}