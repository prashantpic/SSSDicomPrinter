using System;

namespace TheSSS.DICOMViewer.Monitoring.Contracts;

public class PacsConnectionInfoDto
{
    /// <summary>
    /// Identifier for the PACS node (e.g., Configuration ID or internal name).
    /// </summary>
    public string PacsNodeId { get; set; } = string.Empty;

    /// <summary>
    /// AE Title of the PACS node.
    /// </summary>
    public string AETitle { get; set; } = string.Empty;

    /// <summary>
    /// Indicates if the last connectivity check (e.g., C-ECHO) was successful.
    /// </summary>
    public bool IsConnected { get; set; }

    /// <summary>
    /// Timestamp of the last successful connectivity check. Null if never successful or last check failed.
    /// </summary>
    public DateTimeOffset? LastSuccessfulEchoTimestamp { get; set; }

    /// <summary>
    /// Timestamp of the last failed connectivity check. Null if never failed or last check successful.
    /// </summary>
    public DateTimeOffset? LastFailedEchoTimestamp { get; set; }

    /// <summary>
    /// Error message from the last failed connectivity check. Null if successful.
    /// </summary>
    public string? LastEchoErrorMessage { get; set; }

    /// <summary>
    /// Optional: Number of consecutive failures observed for this PACS node.
    /// This might be maintained by the evaluation service or the adapter providing the data.
    /// </summary>
    public int ConsecutiveFailureCount { get; set; } = 0;
}