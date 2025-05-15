using System.Collections.Generic;
using System.IO;

namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object for DICOM C-STORE requests via the gateway.
    /// Specifies target AE, files to send, and transfer syntax options.
    /// </summary>
    public record DicomCStoreRequestDto(
        string TargetAE,
        string TargetHost,
        int TargetPort,
        List<string>? DicomFilePaths, // Paths to DICOM files on disk
        List<Stream>? DicomFileStreams, // In-memory DICOM file streams
        List<string>? PreferredTransferSyntaxes // List of UID strings for preferred transfer syntaxes
    );
}