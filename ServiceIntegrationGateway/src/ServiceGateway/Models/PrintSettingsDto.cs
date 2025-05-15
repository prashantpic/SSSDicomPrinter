namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Represents detailed settings for a print job.
    /// </summary>
    public record PrintSettingsDto
    {
        public string? Orientation { get; init; } // e.g., "Portrait", "Landscape"
        public string? PaperSize { get; init; } // e.g., "A4", "Letter"
        public int? QualityLevel { get; init; } // DPI or abstract quality
        public bool? Collate { get; init; }
        public string? ColorMode { get; init; } // e.g., "Monochrome", "Color"
        public string? DuplexMode { get; init; } // e.g., "Simplex", "DuplexLongEdge", "DuplexShortEdge"

        // Add any other relevant print settings
    }
}