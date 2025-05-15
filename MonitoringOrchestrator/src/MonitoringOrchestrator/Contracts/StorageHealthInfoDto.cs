namespace TheSSS.DICOMViewer.Monitoring.Contracts;

public class StorageHealthInfoDto
{
    public long TotalCapacityBytes { get; set; }
    public long FreeSpaceBytes { get; set; }
    public double UsedPercentage { get; set; }
    public string? StoragePathIdentifier { get; set; } // e.g., path or logical name of the monitored storage
}