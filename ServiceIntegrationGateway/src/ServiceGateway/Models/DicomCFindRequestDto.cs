using System;
using Dicom; // From fo-dicom-core, assuming it's a shared dependency or available through REPO-INFRA
using TheSSS.DICOMViewer.Infrastructure.Models; // Assuming DicomAe is here

namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object for DICOM C-FIND requests via the gateway.
    /// Corresponds to REQ-DNSPI-001.
    /// </summary>
    public record DicomCFindRequestDto
    {
        /// <summary>
        /// The target DICOM Application Entity (SCP) to send the C-FIND request to.
        /// </summary>
        public DicomAe TargetAe { get; init; }

        /// <summary>
        /// The DICOM Query/Retrieve Level (e.g., "PATIENT", "STUDY", "SERIES", "IMAGE").
        /// </summary>
        public string QueryLevel { get; init; }

        /// <summary>
        /// The DicomDataset containing the query keys and match values.
        /// Universal (Tag=value) and Range (Tag=value1-value2) matching should be supported by the SCP.
        /// Wildcard matching (*, ?) for string values.
        /// </summary>
        public DicomDataset QueryKeys { get; init; }

        /// <summary>
        /// Optional: The Calling AE Title for this SCU operation.
        /// If not provided, a default from DicomGatewaySettings might be used.
        /// </summary>
        public string? CallingAeTitle { get; init; }


        public DicomCFindRequestDto(DicomAe targetAe, string queryLevel, DicomDataset queryKeys, string? callingAeTitle = null)
        {
            if (targetAe == null)
                throw new ArgumentNullException(nameof(targetAe));
            if (string.IsNullOrWhiteSpace(queryLevel))
                throw new ArgumentException("Query level cannot be null or whitespace.", nameof(queryLevel));
            if (queryKeys == null)
                throw new ArgumentNullException(nameof(queryKeys));

            TargetAe = targetAe;
            QueryLevel = queryLevel;
            QueryKeys = queryKeys;
            CallingAeTitle = callingAeTitle;
        }
    }
}