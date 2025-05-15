using System;
using System.IO; // For Stream

namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object for submitting print jobs.
    /// Corresponds to REQ-5-001.
    /// </summary>
    public record PrintJobDto
    {
        /// <summary>
        /// The name of the target printer. If null or empty, the default printer from settings or system default is used.
        /// </summary>
        public string? PrinterName { get; init; }

        /// <summary>
        /// The content of the document to be printed as a byte array.
        /// Mutually exclusive with DocumentFilePath and DocumentStream.
        /// </summary>
        public byte[]? DocumentContent { get; init; }

        /// <summary>
        /// The path to a file to be printed. The adapter will read this file.
        /// Mutually exclusive with DocumentContent and DocumentStream.
        /// </summary>
        public string? DocumentFilePath { get; init; }

        /// <summary>
        /// A stream providing the document content. The adapter will read from this stream.
        /// Caller is responsible for managing the stream's lifecycle if passed.
        /// Mutually exclusive with DocumentContent and DocumentFilePath.
        /// </summary>
        [System.Text.Json.Serialization.JsonIgnore] // Streams are not easily serializable by default
        public Stream? DocumentStream { get; init; }


        /// <summary>
        /// The format of the document content (e.g., "PDF", "PNG", "TEXT", "XPS").
        /// This helps the adapter determine how to handle the content.
        /// </summary>
        public string DocumentFormat { get; init; }

        /// <summary>
        /// The number of copies to print. Defaults to 1.
        /// </summary>
        public int Copies { get; init; } = 1;

        /// <summary>
        /// Optional: Title for the print job as it might appear in the print queue.
        /// </summary>
        public string? JobTitle { get; init; }

        /// <summary>
        /// Additional print settings.
        /// </summary>
        public PrintSettings? Settings { get; init; }


        public PrintJobDto(
            string documentFormat,
            byte[]? documentContent = null,
            string? documentFilePath = null,
            Stream? documentStream = null,
            string? printerName = null,
            int copies = 1,
            string? jobTitle = null,
            PrintSettings? settings = null)
        {
            int contentSources = (documentContent != null ? 1 : 0) +
                                 (!string.IsNullOrWhiteSpace(documentFilePath) ? 1 : 0) +
                                 (documentStream != null ? 1 : 0);

            if (contentSources == 0)
                throw new ArgumentException("One of DocumentContent, DocumentFilePath, or DocumentStream must be provided.");
            if (contentSources > 1)
                throw new ArgumentException("Only one of DocumentContent, DocumentFilePath, or DocumentStream can be provided.");

            if (string.IsNullOrWhiteSpace(documentFormat))
                throw new ArgumentException("Document format cannot be null or whitespace.", nameof(documentFormat));
            if (copies < 1)
                throw new ArgumentOutOfRangeException(nameof(copies), "Number of copies must be at least 1.");


            PrinterName = printerName;
            DocumentContent = documentContent;
            DocumentFilePath = documentFilePath;
            DocumentStream = documentStream;
            DocumentFormat = documentFormat;
            Copies = copies;
            JobTitle = jobTitle ?? "ServiceGateway Print Job";
            Settings = settings;
        }
    }

    /// <summary>
    /// Nested record for specific print settings.
    /// </summary>
    public record PrintSettings
    {
        /// <summary>
        /// Print orientation: "Portrait" or "Landscape".
        /// </summary>
        public string? Orientation { get; init; } // "Portrait", "Landscape"

        /// <summary>
        /// Paper size (e.g., "A4", "Letter").
        /// </summary>
        public string? PaperSize { get; init; }

        /// <summary>
        /// Print in color (false for grayscale).
        /// </summary>
        public bool? Color { get; init; }

        /// <summary>
        /// Print quality (e.g., "Draft", "Normal", "High").
        /// </summary>
        public string? Quality { get; init; } // "Draft", "Normal", "High"

        // Add more settings as needed, e.g., duplex, collation
    }
}