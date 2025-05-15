using System;

namespace TheSSS.DICOMViewer.Integration.Configuration
{
    /// <summary>
    /// Configuration settings related to Windows Print API interactions.
    /// </summary>
    public class WindowsPrintSettings
    {
        /// <summary>
        /// Default printer name to use if not specified in the print job request.
        /// If empty, the system's default printer will be used.
        /// </summary>
        public string DefaultPrinterName { get; set; } = string.Empty;

        /// <summary>
        /// Default number of copies for a print job, if not specified.
        /// </summary>
        public int DefaultCopies { get; set; } = 1;

        /// <summary>
        /// Default orientation (e.g., "Portrait", "Landscape").
        /// </summary>
        public string DefaultOrientation { get; set; } = "Portrait";

        // Add other default print options as needed, e.g.:
        // public string DefaultPaperSize { get; set; } = "A4";
        // public bool DefaultGrayscale { get; set; } = false;
    }
}