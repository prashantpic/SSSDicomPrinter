namespace TheSSS.DICOMViewer.Monitoring.Contracts;

public class LicenseStatusInfoDto
{
    public bool IsValid { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string StatusMessage { get; set; } = string.Empty; // e.g., 'Active', 'Expired', 'GracePeriod', 'ValidationFailed', 'NotFound'
    public int? DaysUntilExpiry { get; set; }
}