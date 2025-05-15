using System;
using System.ComponentModel.DataAnnotations;

namespace TheSSS.DICOMViewer.Monitoring.Configuration;

/// <summary>
/// POCO class for alert deduplication configurations.
/// </summary>
public class DeduplicationOptions
{
    /// <summary>
    /// Global flag to enable or disable deduplication.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// The time window during which identical alerts (based on a generated signature)
    /// will be considered duplicates and suppressed.
    /// </summary>
    [Required(ErrorMessage = "DeduplicationWindow is required.")]
    [Range(typeof(TimeSpan), "00:00:01", "30.00:00:00", ErrorMessage = "DeduplicationWindow must be a positive time span.")] // Max 30 days
    public TimeSpan DeduplicationWindow { get; set; } = TimeSpan.FromMinutes(5);
}