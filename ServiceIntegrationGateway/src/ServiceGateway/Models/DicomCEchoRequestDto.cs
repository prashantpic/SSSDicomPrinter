using System;
using TheSSS.DICOMViewer.Infrastructure.Models; // Assuming DicomAe is here

namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object for DICOM C-ECHO requests via the gateway.
    /// Corresponds to REQ-DNSPI-001.
    /// </summary>
    public record DicomCEchoRequestDto
    {
        /// <summary>
        /// The target DICOM Application Entity (SCP) to send the C-ECHO request to.
        /// </summary>
        public DicomAe TargetAe { get; init; }

        /// <summary>
        /// Optional: The Calling AE Title for this SCU operation.
        /// If not provided, a default from DicomGatewaySettings might be used.
        /// </summary>
        public string? CallingAeTitle { get; init; }

        public DicomCEchoRequestDto(DicomAe targetAe, string? callingAeTitle = null)
        {
            if (targetAe == null)
                throw new ArgumentNullException(nameof(targetAe));

            TargetAe = targetAe;
            CallingAeTitle = callingAeTitle;
        }
    }
}