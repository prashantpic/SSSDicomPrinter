namespace TheSSS.DICOMViewer.Integration.Models;

/// <summary>
/// Data Transfer Object representing the specific result from the WindowsPrintAdapter.
/// This DTO is returned by the adapter and may be mapped by the coordinator to a more generic PrintResultDto.
/// </summary>
/// <param name="IsSuccessful">Indicates whether the Windows Print API operation was successful.</param>
/// <param name="StatusMessage">A descriptive message from the Windows Print adapter regarding the operation's outcome.</param>
/// <param name="SystemPrintJobId">An optional job identifier returned by the Windows print spooler or API.</param>
public record WindowsPrintResultDto(
    bool IsSuccessful,
    string StatusMessage,
    string? SystemPrintJobId
);