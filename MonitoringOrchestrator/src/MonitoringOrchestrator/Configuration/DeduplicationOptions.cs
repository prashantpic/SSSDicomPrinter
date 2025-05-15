namespace TheSSS.DICOMViewer.Monitoring.Configuration;

public class DeduplicationOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether alert deduplication is enabled.
    /// Default is true.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the time window for deduplicating alerts.
    /// Alerts considered identical (based on content signature) occurring within this window will be suppressed.
    /// Default is 15 minutes.
    /// </summary>
    public TimeSpan DeduplicationWindow { get; set; } = TimeSpan.FromMinutes(15);
}