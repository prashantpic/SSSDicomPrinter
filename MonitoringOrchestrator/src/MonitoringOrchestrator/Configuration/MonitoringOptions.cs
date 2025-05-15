using System;

namespace TheSSS.DICOMViewer.Monitoring.Configuration;

/// <summary>
/// Holds global configuration settings for the monitoring service.
/// </summary>
public class MonitoringOptions
{
    /// <summary>
    /// Gets or sets the interval between full system health checks.
    /// Default is 5 minutes.
    /// </summary>
    public TimeSpan SystemHealthCheckInterval { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets or sets a value indicating whether monitoring is globally enabled.
    /// Default is true.
    /// </summary>
    public bool IsMonitoringEnabled { get; set; } = true;
}