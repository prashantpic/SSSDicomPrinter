namespace TheSSS.DICOMViewer.Monitoring.Contracts;

/// <summary>
/// Contains information about the application's license status.
/// </summary>
/// <param name="IsValid">A value indicating whether the license is currently valid.</param>
/// <param name="ExpiryDate">The expiry date of the license, if applicable.</param>
/// <param name="StatusMessage">A message describing the license status (e.g., 'Active', 'Expired', 'GracePeriod').</param>
/// <param name="DaysUntilExpiry">The number of days remaining until the license expires, if applicable.</param>
public record LicenseStatusInfoDto(
    bool IsValid,
    DateTime? ExpiryDate,
    string StatusMessage,
    int? DaysUntilExpiry
);