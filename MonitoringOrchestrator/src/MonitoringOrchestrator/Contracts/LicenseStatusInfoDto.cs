using System;

namespace TheSSS.DICOMViewer.Monitoring.Contracts;

public class LicenseStatusInfoDto
{
    /// <summary>
    /// Indicates if the application license is currently valid.
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// The expiration date of the license, if applicable. Null for perpetual licenses or if invalid.
    /// </summary>
    public DateTimeOffset? ExpiryDate { get; set; }

    /// <summary>
    /// A human-readable status message (e.g., 'Active', 'Expired', 'GracePeriod', 'ValidationFailed', 'NotActivated').
    /// </summary>
    public string StatusMessage { get; set; } = string.Empty;

    /// <summary>
    /// Number of days until expiry, if applicable and valid. Null otherwise.
    /// Calculated as (ExpiryDate - UtcNow).Days.
    /// </summary>
    public int? DaysUntilExpiry => IsValid && ExpiryDate.HasValue ? (int)(ExpiryDate.Value - DateTimeOffset.UtcNow).TotalDays : (int?)null;
}