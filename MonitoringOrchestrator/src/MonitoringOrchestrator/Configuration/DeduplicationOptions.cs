using System;

namespace TheSSS.DICOMViewer.Monitoring.Configuration;

/// <summary>
/// Holds configuration settings for alert deduplication.
/// </summary>
public class DeduplicationOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether alert deduplication is enabled.
    /// Default is true.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the window of time to look back for identical alerts to consider an incoming alert a duplicate.
    /// Default is 5 minutes.
    /// </summary>
    public TimeSpan DeduplicationWindow { get; set; } = TimeSpan.FromMinutes(5);
}