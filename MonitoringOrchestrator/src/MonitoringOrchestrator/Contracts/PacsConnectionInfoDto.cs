namespace TheSSS.DICOMViewer.Monitoring.Contracts;

/// <summary>
/// Contains information about the connectivity status of a PACS node.
/// </summary>
/// <param name="PacsNodeId">The identifier or AE Title of the PACS node.</param>
/// <param name="IsConnected">A value indicating whether the PACS node is currently connected (e.g., last C-ECHO was successful).</param>
/// <param name="LastSuccessfulEchoTimestamp">The timestamp of the last successful C-ECHO, if any.</param>
/// <param name="LastFailedEchoTimestamp">The timestamp of the last failed C-ECHO, if any.</param>
/// <param name="LastEchoErrorMessage">The error message from the last failed C-ECHO, if applicable.</param>
public record PacsConnectionInfoDto(
    string PacsNodeId,
    bool IsConnected,
    DateTimeOffset? LastSuccessfulEchoTimestamp,
    DateTimeOffset? LastFailedEchoTimestamp,
    string? LastEchoErrorMessage
);