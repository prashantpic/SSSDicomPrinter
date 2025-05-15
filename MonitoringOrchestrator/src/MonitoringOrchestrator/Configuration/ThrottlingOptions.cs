using System;
using System.ComponentModel.DataAnnotations;

namespace TheSSS.DICOMViewer.Monitoring.Configuration;

/// <summary>
/// POCO class for alert throttling configurations.
/// </summary>
public class ThrottlingOptions
{
    /// <summary>
    /// Global flag to enable or disable throttling.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// The default time window during which alerts for the same rule/source (or same alert signature)
    /// will be throttled after an initial alert is sent.
    /// </summary>
    [Required(ErrorMessage = "DefaultThrottleWindow is required.")]
    [Range(typeof(TimeSpan), "00:00:01", "30.00:00:00", ErrorMessage = "DefaultThrottleWindow must be a positive time span.")] // Max 30 days
    public TimeSpan DefaultThrottleWindow { get; set; } = TimeSpan.FromHours(1);

    /// <summary>
    /// The maximum number of alerts allowed for a given rule/source within its throttle window
    /// before further alerts are suppressed. A value of 1 means only the first alert gets through during the window.
    /// Default is 1.
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "MaxAlertsPerWindow must be at least 1.")]
    public int MaxAlertsPerWindow { get; set; } = 1;

    // Optional: Rule-specific overrides for throttling behavior.
    // public Dictionary<string, RuleThrottlingOverride> RuleOverrides { get; set; } = new Dictionary<string, RuleThrottlingOverride>();
}

/* Example for Rule-specific override (if implemented)
public class RuleThrottlingOverride
{
    [Required]
    public TimeSpan ThrottleWindow { get; set; }
    [Range(1, int.MaxValue)]
    public int MaxAlertsPerWindow { get; set; }
}
*/