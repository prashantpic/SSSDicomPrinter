using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using System.Text.Json;

namespace TheSSS.DICOMViewer.Monitoring.Mappers;

public static class HealthReportMapper
{
    public static NotificationPayloadDto ToNotificationPayload(
        AlertContextDto alertContext,
        string channelType,
        List<string>? recipientDetails,
        string defaultSourceComponent)
    {
        return new NotificationPayloadDto
        {
            Title = $"[{alertContext.Severity}] {alertContext.TriggeredRuleName}",
            Body = FormatAlertBody(alertContext),
            Severity = alertContext.Severity,
            Timestamp = alertContext.Timestamp,
            TargetChannelType = channelType,
            RecipientDetails = recipientDetails,
            SourceComponent = alertContext.SourceComponent ?? defaultSourceComponent,
            TriggeredRuleName = alertContext.TriggeredRuleName
        };
    }

    private static string FormatAlertBody(AlertContextDto alertContext)
    {
        return $@"Source: {alertContext.SourceComponent}
Message: {alertContext.Message}
Timestamp: {alertContext.Timestamp:O}
Details: {JsonSerializer.Serialize(alertContext.RawData, new JsonSerializerOptions { WriteIndented = true })}";
    }

    public static OverallHealthStatus DetermineOverallStatus(HealthReportDto report)
    {
        if (report.DatabaseHealth?.IsConnected == false) return OverallHealthStatus.Error;
        if (report.LicenseStatus?.IsValid == false) return OverallHealthStatus.Error;
        if (report.StorageHealth?.UsedPercentage > 90) return OverallHealthStatus.Warning;
        return OverallHealthStatus.Healthy;
    }
}