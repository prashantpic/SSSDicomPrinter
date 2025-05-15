namespace TheSSS.DICOMViewer.Monitoring.Contracts;

public class AutomatedTaskStatusInfoDto
{
    public string TaskName { get; set; } = string.Empty;
    public DateTime? LastRunTimestamp { get; set; }
    public string LastRunStatus { get; set; } = string.Empty; // e.g., "Success", "Failed", "Running", "Unknown", "NotRunYet"
    public string? ErrorMessage { get; set; }
    public DateTime? NextRunTimestamp { get; set; }
}