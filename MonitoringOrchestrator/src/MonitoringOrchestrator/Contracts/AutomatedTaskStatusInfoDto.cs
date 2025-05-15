using System;

namespace TheSSS.DICOMViewer.Monitoring.Contracts;

public class AutomatedTaskStatusInfoDto
{
    /// <summary>
    /// The name or identifier of the automated task (e.g., "DataPurge", "DatabaseBackup", "PacsSync").
    /// </summary>
    public string TaskName { get; set; } = string.Empty;

    /// <summary>
    /// Timestamp when the task last ran. Null if never run.
    /// </summary>
    public DateTimeOffset? LastRunTimestamp { get; set; }

    /// <summary>
    /// Status of the last run (e.g., "Success", "Failed", "Running", "Skipped", "Pending").
    /// </summary>
    public string LastRunStatus { get; set; } = "Unknown";

    /// <summary>
    /// Error message if the last run failed. Null otherwise.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Timestamp when the task is scheduled to run next. Null if not scheduled or task is manually triggered.
    /// </summary>
    public DateTimeOffset? NextRunTimestamp { get; set; }
}