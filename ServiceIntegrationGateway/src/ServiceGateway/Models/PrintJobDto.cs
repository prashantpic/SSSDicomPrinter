using System.IO;

namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object for submitting print jobs.
    /// Includes document content/stream and printer-specific settings.
    /// </summary>
    public record PrintJobDto(
        string? PrinterName, // If null, use default printer from settings or OS
        Stream DocumentStream, // The content to print
        string DocumentName, // Name of the document/job
        string DocumentFormat, // e.g., "XPS", "PDF", "ImageStream" - hints for the adapter
        int Copies,
        PrintSettingsDto? Settings // Specific print settings for this job
    )
    {
        public PrintJobDto(Stream documentStream, string documentName, string documentFormat, int copies = 1)
            : this(null, documentStream, documentName, documentFormat, copies, null)
        {
        }
    }
}