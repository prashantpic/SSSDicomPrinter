using System;
using System.Collections.Generic;
using TheSSS.DICOMViewer.Infrastructure.Models; // Assuming DicomAe is here

namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object for DICOM C-STORE requests via the gateway.
    /// Corresponds to REQ-DNSPI-001.
    /// </summary>
    public record DicomCStoreRequestDto
    {
        /// <summary>
        /// The target DICOM Application Entity (SCP) to send the instances to.
        /// </summary>
        public DicomAe TargetAe { get; init; }

        /// <summary>
        /// A list of full file paths to the DICOM files to be sent.
        /// Alternative: could be List<byte[]> for in-memory DICOM data, or List<Stream>.
        /// For simplicity, using file paths as per SDS.
        /// </summary>
        public List<string> DicomFilePaths { get; init; } = new List<string>();

        /// <summary>
        /// Optional: A list of preferred DICOM Transfer Syntax UIDs for negotiation.
        /// If null or empty, the underlying DICOM client's defaults will be used.
        /// </summary>
        public List<string>? PreferredTransferSyntaxes { get; init; }

        /// <summary>
        /// Optional: The Calling AE Title for this SCU operation.
        /// If not provided, a default from DicomGatewaySettings might be used.
        /// </summary>
        public string? CallingAeTitle { get; init; }

        public DicomCStoreRequestDto(DicomAe targetAe, List<string> dicomFilePaths, string? callingAeTitle = null, List<string>? preferredTransferSyntaxes = null)
        {
            if (targetAe == null)
                throw new ArgumentNullException(nameof(targetAe));
            if (dicomFilePaths == null || dicomFilePaths.Count == 0 || dicomFilePaths.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("DICOM file paths list cannot be null, empty, or contain invalid paths.", nameof(dicomFilePaths));

            TargetAe = targetAe;
            DicomFilePaths = dicomFilePaths;
            CallingAeTitle = callingAeTitle;
            PreferredTransferSyntaxes = preferredTransferSyntaxes;
        }
    }
}