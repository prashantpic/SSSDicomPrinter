namespace TheSSS.DICOMViewer.Monitoring.Contracts;

public class DatabaseConnectivityInfoDto
{
    public bool IsConnected { get; set; }
    public DateTime LastCheckTimestamp { get; set; }
    public string? ErrorMessage { get; set; }
    public long? LatencyMs { get; set; }
}