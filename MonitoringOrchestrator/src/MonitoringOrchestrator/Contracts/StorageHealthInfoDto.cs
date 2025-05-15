namespace TheSSS.DICOMViewer.Monitoring.Contracts;

public class StorageHealthInfoDto
{
    public long TotalCapacityBytes { get; set; }
    public long FreeSpaceBytes { get; set; }
    public double UsedPercentage { get; set; }
    public string? StoragePathIdentifier { get; set; }
}