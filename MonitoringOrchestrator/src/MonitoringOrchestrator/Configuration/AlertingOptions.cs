using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Monitoring.Configuration;

/// <summary>
/// Holds all configurable settings related to alert generation and dispatch.
/// </summary>
public class AlertingOptions
{
    /// <summary>
    /// Gets or sets the list of configured alert rules.
    /// </summary>
    public List<AlertRule> Rules { get; set; } = new();

    /// <summary>
    /// Gets or sets the list of configurations for alert channels.
    /// </summary>
    public List<AlertChannelSetting> Channels { get; set; } = new();

    /// <summary>
    /// Gets or sets the configuration for alert throttling.
    /// </summary>
    public ThrottlingOptions Throttling { get; set; } = new();

    /// <summary>
    /// Gets or sets the configuration for alert deduplication.
    /// </summary>
    public DeduplicationOptions Deduplication { get; set; } = new();
}