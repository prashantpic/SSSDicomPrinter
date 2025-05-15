namespace TheSSS.DICOMViewer.Monitoring.Contracts;

/// <summary>
/// Contains information about the database connectivity status.
/// </summary>
/// <param name="IsConnected">A value indicating whether the database connection is active.</param>
/// <param name="LastCheckTimestamp">The timestamp of the last connectivity check.</param>
/// <param name="ErrorMessage">An error message if the connection check failed, otherwise null.</param>
/// <param name="LatencyMs">The latency of the connection check in milliseconds, if successful and applicable.</param>
public record DatabaseConnectivityInfoDto(
    bool IsConnected,
    DateTimeOffset LastCheckTimestamp,
    string? ErrorMessage,
    long? LatencyMs
);