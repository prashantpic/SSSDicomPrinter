namespace TheSSS.DICOMViewer.Monitoring.Configuration;

public class ThrottlingOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether alert throttling is enabled.
    /// Default is true.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets or sets the default time window for throttling alerts.
    /// If an alert for the same rule/source is triggered within this window, it might be suppressed.
    /// Default is 30 minutes.
    /// </summary>
    public TimeSpan DefaultThrottleWindow { get; set; } = TimeSpan.FromMinutes(30);

    /// <summary>
    /// Gets or sets the maximum number of alerts for the same rule/source allowed within the throttle window.
    /// Default is 1 (meaning after the first alert, subsequent identical alerts are throttled for the window duration).
    /// Note: The DefaultAlertThrottlingStrategy implements a simpler "minimum interval" based on this window.
    /// More sophisticated "max per window" would require more complex state management.
    /// </summary>
    public int MaxAlertsPerWindow { get; set; } = 1;
}