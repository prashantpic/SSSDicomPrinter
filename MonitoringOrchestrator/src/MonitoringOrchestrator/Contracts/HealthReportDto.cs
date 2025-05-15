namespace TheSSS.DICOMViewer.Monitoring.Contracts;

public enum OverallHealthStatus
{
    Healthy,
    Warning,
    Error,
    Unknown
}

public class HealthReportDto
{
    public OverallHealthStatus OverallStatus { get; set; }
    public DateTime Timestamp { get; set; }
    public StorageHealthInfoDto? StorageHealth { get; set; }
    public DatabaseConnectivityInfoDto? DatabaseHealth { get; set; }
    public List<PacsConnectionInfoDto>? PacsConnections { get; set; }
    public LicenseStatusInfoDto? LicenseStatus { get; set; }
    public SystemErrorInfoSummaryDto? SystemErrorSummary { get; set; }
    public List<AutomatedTaskStatusInfoDto>? AutomatedTaskStatuses { get; set; }
}