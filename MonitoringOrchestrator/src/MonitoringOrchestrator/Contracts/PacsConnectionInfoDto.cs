namespace TheSSS.DICOMViewer.Monitoring.Contracts;

public class PacsConnectionInfoDto
{
    public string PacsNodeId { get; set; } = string.Empty;
    public bool IsConnected { get; set; }
    public DateTime? LastSuccessfulEchoTimestamp { get; set; }
    public DateTime? LastFailedEchoTimestamp { get; set; }
    public string? LastEchoErrorMessage { get; set; }
}