namespace TheSSS.DICOMViewer.Integration.Configuration
{
    /// <summary>
    /// Configuration settings related to Windows Print API interactions.
    /// Such as default printer behavior or specific print job defaults managed by the gateway.
    /// </summary>
    public class WindowsPrintSettings
    {
        public string? DefaultPrinterName { get; set; }
        // Add other print-specific settings if needed, e.g.:
        // public string DefaultPaperSize { get; set; } = "A4";
        // public string DefaultOrientation { get; set; } = "Portrait";
        // public int DefaultCopies { get; set; } = 1;
    }
}