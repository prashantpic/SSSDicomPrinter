using System;

namespace TheSSS.DICOMViewer.Monitoring.Configuration;

/// <summary>
/// Holds configuration settings for alert throttling.
/// </summary>
public class ThrottlingOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether alert throttling is enabled.
    /// Default is true.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the default window of time during which alerts of the same kind are throttled after an initial alert.
    /// Default is 60 minutes.
    /// </summary>
    public TimeSpan DefaultThrottleWindow { get; set; } = TimeSpan.FromMinutes(60);

    /// <summary>
    /// Gets or sets the maximum number of alerts of the same kind to be dispatched within the throttle window.
    /// Default is 1 (meaning after the first alert, subsequent identical alerts are suppressed for the duration of the window).
    /// </summary>
    public int MaxAlertsPerWindow { get; set; } = 1;
}