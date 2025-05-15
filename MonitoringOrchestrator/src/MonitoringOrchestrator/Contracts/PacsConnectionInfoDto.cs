namespace TheSSS.DICOMViewer.Monitoring.Contracts;

public class PacsConnectionInfoDto
{
    public string PacsNodeId { get; set; } = string.Empty; // Could be AE Title or another unique identifier
    public string? AETitle { get; set; }
    public bool IsConnected { get; set; }
    public DateTime? LastSuccessfulEchoTimestamp { get; set; }
    public DateTime? LastFailedEchoTimestamp { get; set; }
    public string? LastEchoErrorMessage { get; set; }
    public int ConsecutiveFailedChecks { get; set; } // Useful for alert rules
}