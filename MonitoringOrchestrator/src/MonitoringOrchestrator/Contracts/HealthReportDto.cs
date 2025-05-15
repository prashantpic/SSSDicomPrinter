namespace TheSSS.DICOMViewer.Monitoring.Contracts;

public enum OverallHealthStatus
{
    Unknown,
    Healthy,
    Warning,
    Critical, // For more severe issues than Warning
    Error     // For critical failures impacting core functionality
}

public class HealthReportDto
{
    public OverallHealthStatus OverallStatus { get; set; } = OverallHealthStatus.Unknown;
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public StorageHealthInfoDto? StorageHealth { get; set; }
    public DatabaseConnectivityInfoDto? DatabaseHealth { get; set; }
    public IEnumerable<PacsConnectionInfoDto>? PacsConnections { get; set; }
    public LicenseStatusInfoDto? LicenseStatus { get; set; }
    public SystemErrorInfoSummaryDto? SystemErrorSummary { get; set; }
    public IEnumerable<AutomatedTaskStatusInfoDto>? AutomatedTaskStatuses { get; set; }
    // Add other relevant health metrics as properties
}