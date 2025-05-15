namespace TheSSS.DICOMViewer.Integration.Configuration;

/// <summary>
/// Configuration settings related to Windows Print API interactions.
/// Example: default printer behavior or specific print job defaults managed by the gateway.
/// </summary>
public class WindowsPrintSettings
{
    // REQ-5-001: Coordinates Windows Print API.
    // This class is a placeholder for any gateway-level configurations specific to printing.
    // Specific job settings (printer name, copies) are expected in PrintJobDto.
    // Could include things like:
    // public string DefaultPrinterName { get; set; } // If application needs a fallback.
    // public int MaxRetriesForSpooler { get; set; } // If we add retries at this level.

    /// <summary>
    /// Gets or sets the key for retrieving the specific resilience policy for Windows Print operations from IResiliencePolicyProvider, if applicable.
    /// Printing is often a local operation; resilience might be less about network and more about local spooler issues.
    /// </summary>
    public string PolicyKey { get; set; } = "WindowsPrintPolicy"; // Example key
}