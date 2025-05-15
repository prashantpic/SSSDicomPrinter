namespace TheSSS.DICOMViewer.Monitoring.Configuration;

public class AlertingOptions
{
    public const string SectionName = "Alerting";
    public List<AlertRule> Rules { get; set; } = new();
    public List<AlertChannelSetting> Channels { get; set; } = new();
    public ThrottlingOptions Throttling { get; set; } = new();
    public DeduplicationOptions Deduplication { get; set; } = new();
    public string DefaultAlertSourceComponent { get; set; } = "MonitoringOrchestrator";
}