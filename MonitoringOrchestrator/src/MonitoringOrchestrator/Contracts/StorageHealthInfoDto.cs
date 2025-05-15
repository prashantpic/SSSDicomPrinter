namespace TheSSS.DICOMViewer.Monitoring.Contracts;

public class StorageHealthInfoDto
{
    /// <summary>
    /// Identifier or path of the monitored storage (e.g., C:\DICOMData, /mnt/dicom_repo).
    /// Optional if there's only one primary storage.
    /// </summary>
    public string? Path { get; set; }

    /// <summary>
    /// Total storage capacity in bytes.
    /// </summary>
    public long TotalCapacityBytes { get; set; }

    /// <summary>
    /// Free space available in bytes.
    /// </summary>
    public long FreeSpaceBytes { get; set; }

    /// <summary>
    /// Percentage of storage used. Calculated as ((TotalCapacityBytes - FreeSpaceBytes) / TotalCapacityBytes) * 100.
    /// Returns 0 if TotalCapacityBytes is 0 to avoid division by zero.
    /// </summary>
    public double UsedPercentage => TotalCapacityBytes > 0 ? (((double)TotalCapacityBytes - FreeSpaceBytes) / TotalCapacityBytes) * 100.0 : 0.0;

    /// <summary>
    /// Optional status message related to storage health (e.g., "Disk Full", "Nearing Capacity").
    /// </summary>
    public string? StatusMessage { get; set; }
}