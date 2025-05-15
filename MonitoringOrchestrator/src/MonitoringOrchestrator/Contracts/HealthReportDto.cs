using System;
using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Monitoring.Contracts;

public class HealthReportDto
{
    /// <summary>
    /// Overall system health status (e.g., Healthy, Warning, Error, Degraded).
    /// </summary>
    public string OverallStatus { get; set; } = "Unknown";

    /// <summary>
    /// Timestamp when the report was generated.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Storage health information.
    /// </summary>
    public StorageHealthInfoDto? StorageInfo { get; set; }

    /// <summary>
    /// Database connectivity status.
    /// </summary>
    public DatabaseConnectivityInfoDto? DatabaseConnectivity { get; set; }

    /// <summary>
    /// Collection of PACS node connection statuses.
    /// </summary>
    public IEnumerable<PacsConnectionInfoDto>? PacsConnections { get; set; }

    /// <summary>
    /// Application license status.
    /// </summary>
    public LicenseStatusInfoDto? LicenseStatus { get; set; }

    /// <summary>
    /// Summary of critical system errors.
    /// </summary>
    public SystemErrorInfoSummaryDto? SystemErrorSummary { get; set; }

    /// <summary>
    /// Statuses of automated background tasks.
    /// </summary>
    public IEnumerable<AutomatedTaskStatusInfoDto>? AutomatedTaskStatuses { get; set; }

    /// <summary>
    /// List of detailed messages, typically warnings or errors encountered during aggregation.
    /// </summary>
    public List<string> DetailedMessages { get; set; } = new List<string>();

    /// <summary>
    /// Optional dictionary for any additional, non-standardized data.
    /// </summary>
    public Dictionary<string, object?> AdditionalData { get; set; } = new Dictionary<string, object?>();
}