using System.Collections.Generic;
using System.IO;

namespace TheSSS.DICOMViewer.Integration.Models;

/// <summary>
/// Data Transfer Object for DICOM C-STORE requests via the gateway.
/// Specifies target Application Entity (AE), files/streams to send, and transfer syntax options.
/// </summary>
/// <param name="TargetAe">The target DICOM Application Entity to send the data to.</param>
/// <param name="DicomFilePaths">An optional list of file paths for DICOM instances to be sent.
/// Use this if reading files directly from disk.</param>
/// <param name="DicomFileStreams">An optional list of streams, each representing a DICOM instance to be sent.
/// Use this for in-memory DICOM data. The caller is responsible for managing stream lifecycle if not consumed fully.</param>
/// <param name="PreferredTransferSyntaxes">An optional list of preferred transfer syntax UIDs for negotiation.</param>
public record DicomCStoreRequestDto(
    DicomAETarget TargetAe,
    List<string>? DicomFilePaths,
    List<Stream>? DicomFileStreams, // Added as per initial SDS
    List<string>? PreferredTransferSyntaxes
)
{
    /// <summary>
    /// Validates that either DicomFilePaths or DicomFileStreams (or both) are provided.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if no DICOM data sources are provided.</exception>
    public void Validate()
    {
        if ((DicomFilePaths == null || DicomFilePaths.Count == 0) &&
            (DicomFileStreams == null || DicomFileStreams.Count == 0))
        {
            throw new ArgumentException("Either DicomFilePaths or DicomFileStreams must be provided for a C-STORE operation.");
        }
    }
}

/// <summary>
/// Represents a DICOM Application Entity (AE) target for network operations.
/// </summary>
/// <param name="AeTitle">The Application Entity Title of the target.</param>
/// <param name="Host">The hostname or IP address of the target.</param>
/// <param name="Port">The port number of the target.</param>
public record DicomAETarget(
    string AeTitle,
    string Host,
    int Port
);