using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Monitoring.Contracts;

/// <summary>
/// Represents the aggregated health state of the system.
/// </summary>
/// <param name="Status">The overall system status.</param>
/// <param name="Timestamp">The timestamp when the health report was generated.</param>
/// <param name="StorageInfo">Health information for the storage system, if available.</param>
/// <param name="DatabaseConnectivity">Health information for database connectivity, if available.</param>
/// <param name="PacsStatuses">A list of health information for PACS node connections, if available.</param>
/// <param name="LicenseStatus">Health information for the application license, if available.</param>
/// <param name="SystemErrorSummary">A summary of system errors, if available.</param>
/// <param name="AutomatedTaskStatuses">A list of statuses for automated tasks, if available.</param>
/// <param name="Details">Optional dictionary for any other miscellaneous health details.</param>
public record HealthReportDto(
    SystemStatus Status,
    DateTimeOffset Timestamp,
    StorageHealthInfoDto? StorageInfo,
    DatabaseConnectivityInfoDto? DatabaseConnectivity,
    List<PacsConnectionInfoDto>? PacsStatuses,
    LicenseStatusInfoDto? LicenseStatus,
    SystemErrorInfoSummaryDto? SystemErrorSummary,
    List<AutomatedTaskStatusInfoDto>? AutomatedTaskStatuses,
    Dictionary<string, object>? Details = null
);