namespace TheSSS.DICOMViewer.Integration.Models;

/// <summary>
/// Data Transfer Object representing the result of a print job submission as exposed by the gateway.
/// </summary>
/// <param name="IsSuccessful">Indicates whether the print job was successfully submitted to the print subsystem.</param>
/// <param name="Message">A message providing details about the submission status (e.g., "Print job submitted", "Printer not found").</param>
/// <param name="PrintJobId">An optional identifier for the print job assigned by the print subsystem, if available.</param>
public record PrintResultDto(
    bool IsSuccessful,
    string Message,
    string? PrintJobId
);