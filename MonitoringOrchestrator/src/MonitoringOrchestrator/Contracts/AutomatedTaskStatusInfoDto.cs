namespace TheSSS.DICOMViewer.Monitoring.Contracts;

/// <summary>
/// Contains status information for an automated background task.
/// </summary>
/// <param name="TaskName">The name of the automated task.</param>
/// <param name="LastRunTimestamp">The timestamp of the last time the task was run, if applicable.</param>
/// <param name="LastRunStatus">The status of the last run (e.g., "Success", "Failed", "Running").</param>
/// <param name="ErrorMessage">An error message if the last run failed, otherwise null.</param>
/// <param name="NextRunTimestamp">The timestamp of the next scheduled run, if applicable.</param>
public record AutomatedTaskStatusInfoDto(
    string TaskName,
    DateTimeOffset? LastRunTimestamp,
    string LastRunStatus,
    string? ErrorMessage,
    DateTimeOffset? NextRunTimestamp
);