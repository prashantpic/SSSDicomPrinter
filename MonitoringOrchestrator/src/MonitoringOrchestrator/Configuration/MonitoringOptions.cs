namespace TheSSS.DICOMViewer.Monitoring.Configuration;

public class MonitoringOptions
{
    public const string SectionName = "Monitoring";

    /// <summary>
    /// Gets or sets the interval at which system health checks are performed.
    /// Default is 5 minutes.
    /// </summary>
    public TimeSpan SystemHealthCheckInterval { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Gets or sets a value indicating whether overall monitoring is enabled.
    /// Default is true.
    /// </summary>
    public bool IsMonitoringEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the lookback period for querying critical system errors.
    /// Default is 24 hours.
    /// </summary>
    public TimeSpan CriticalErrorLookbackPeriod { get; set; } = TimeSpan.FromHours(24);
}