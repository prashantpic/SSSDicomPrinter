using System.IO;

namespace TheSSS.DICOMViewer.Integration.Models;

/// <summary>
/// Data Transfer Object for submitting a print job.
/// Specifies the document to be printed, target printer, and various print settings.
/// </summary>
/// <param name="PrinterName">The name of the target printer. If null or empty, the system's default printer may be used.</param>
/// <param name="DocumentStream">A stream containing the content of the document to be printed. The caller is responsible for disposing of this stream after the print operation if necessary.</param>
/// <param name="DocumentFileName">An optional name for the document, which might appear in the print queue.</param>
/// <param name="Copies">The number of copies to print. Defaults to 1.</param>
/// <param name="IsDuplex">A value indicating whether to print on both sides of the paper (duplex). Defaults to false.</param>
/// <param name="Orientation">The print orientation (e.g., "Portrait", "Landscape"). Defaults to "Portrait".</param>
/// <param name="PaperSize">The paper size (e.g., "A4", "Letter"). Platform-dependent; exact values may need to conform to printer capabilities.</param>
public record PrintJobDto(
    string? PrinterName,
    Stream DocumentStream, // Changed from DocumentContent to DocumentStream as per initial SDS
    string? DocumentFileName,
    int Copies = 1,
    bool IsDuplex = false, // Changed from Duplex
    string Orientation = "Portrait",
    string? PaperSize = null // e.g., "A4", "Letter". Can be null for printer default.
);