namespace TheSSS.DICOMViewer.Monitoring.Configuration;

public class AlertingOptions
{
    public const string SectionName = "Alerting";

    /// <summary>
    /// Gets or sets a list of defined alert rules.
    /// </summary>
    public List<AlertRule> Rules { get; set; } = new List<AlertRule>();

    /// <summary>
    /// Gets or sets a list of configurations for available alerting channels.
    /// </summary>
    public List<AlertChannelSetting> Channels { get; set; } = new List<AlertChannelSetting>();

    /// <summary>
    /// Gets or sets the configuration for alert throttling.
    /// </summary>
    public ThrottlingOptions Throttling { get; set; } = new ThrottlingOptions();

    /// <summary>
    /// Gets or sets the configuration for alert deduplication.
    /// </summary>
    public DeduplicationOptions Deduplication { get; set; } = new DeduplicationOptions();

    /// <summary>
    /// Gets or sets the default source component name used in alerts if not overridden.
    /// Default is "MonitoringOrchestrator".
    /// </summary>
    public string DefaultAlertSourceComponent { get; set; } = "MonitoringOrchestrator";

    /// <summary>
    /// Gets or sets a global flag to enable or disable all alerting.
    /// Default is true.
    /// </summary>
    public bool IsEnabled { get; set; } = true;
}