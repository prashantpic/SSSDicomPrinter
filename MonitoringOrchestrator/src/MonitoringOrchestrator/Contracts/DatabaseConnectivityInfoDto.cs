using System;

namespace TheSSS.DICOMViewer.Monitoring.Contracts;

public class DatabaseConnectivityInfoDto
{
    /// <summary>
    /// Indicates if a connection to the database was successful.
    /// </summary>
    public bool IsConnected { get; set; }

    /// <summary>
    /// Timestamp of the last connectivity check.
    /// </summary>
    public DateTimeOffset LastCheckTimestamp { get; set; }

    /// <summary>
    /// Error message if the connection failed. Null if successful.
    /// </summary>
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// Optional latency in milliseconds for the check. Null if check failed or not measured.
    /// </summary>
    public long? LatencyMs { get; set; }
}