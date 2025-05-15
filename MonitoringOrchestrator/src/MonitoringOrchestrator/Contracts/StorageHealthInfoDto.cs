namespace TheSSS.DICOMViewer.Monitoring.Contracts;

/// <summary>
/// Contains health information for a storage system.
/// </summary>
/// <param name="TotalCapacityBytes">The total capacity of the storage in bytes.</param>
/// <param name="FreeSpaceBytes">The available free space on the storage in bytes.</param>
/// <param name="UsedPercentage">The percentage of storage space currently used.</param>
/// <param name="Path">The path or identifier of the monitored storage.</param>
public record StorageHealthInfoDto(
    long TotalCapacityBytes,
    long FreeSpaceBytes,
    double UsedPercentage,
    string Path
);