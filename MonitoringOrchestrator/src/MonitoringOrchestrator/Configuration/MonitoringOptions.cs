using System;
using System.ComponentModel.DataAnnotations;

namespace TheSSS.DICOMViewer.Monitoring.Configuration;

/// <summary>
/// POCO class for global monitoring settings, bindable from configuration.
/// </summary>
public class MonitoringOptions
{
    /// <summary>
    /// Interval between full system health checks.
    /// Default is 5 minutes. Must be a positive time span.
    /// </summary>
    [Required(ErrorMessage = "SystemHealthCheckInterval is required.")]
    [Range(typeof(TimeSpan), "00:00:01", "23:59:59", ErrorMessage = "SystemHealthCheckInterval must be between 1 second and 24 hours.")]
    public TimeSpan SystemHealthCheckInterval { get; set; } = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Global flag to enable or disable monitoring.
    /// If false, the SystemHealthMonitorWorker will not perform checks.
    /// </summary>
    public bool IsMonitoringEnabled { get; set; } = true;

    /// <summary>
    /// Lookback period for querying critical system errors from logs.
    /// Default is 24 hours. Must be a positive time span.
    /// </summary>
    [Required(ErrorMessage = "ErrorLogLookbackPeriod is required.")]
    [Range(typeof(TimeSpan), "00:01:00", "30.00:00:00", ErrorMessage = "ErrorLogLookbackPeriod must be between 1 minute and 30 days.")] // Example range
    public TimeSpan ErrorLogLookbackPeriod { get; set; } = TimeSpan.FromHours(24);
}