using System;
using System.Collections.Generic;
using Dicom; // From fo-dicom-core
using TheSSS.DICOMViewer.Infrastructure.Models; // Assuming DicomAe is here

namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object for DICOM C-MOVE requests via the gateway.
    /// Corresponds to REQ-DNSPI-001.
    /// </summary>
    public record DicomCMoveRequestDto
    {
        /// <summary>
        /// The source DICOM Application Entity (SCP Query/Retrieve Server) from which to move the data.
        /// </summary>
        public DicomAe SourceAe { get; init; }

        /// <summary>
        /// The AE Title of the destination where the DICOM instances should be sent.
        /// This is often the AE Title of the application initiating the C-MOVE.
        /// </summary>
        public string DestinationAeTitle { get; init; }

        /// <summary>
        /// A list of Study Instance UIDs to be moved.
        /// Depending on the C-MOVE implementation, this could also be Series or SOP Instance UIDs,
        /// often specified within a DicomDataset. For simplicity, using a list of UIDs here.
        /// The SDS implies `List<string> StudyInstanceUids`.
        /// </summary>
        public List<string> StudyInstanceUids { get; init; } = new List<string>();

        // Alternatively, a DicomDataset could be used for more complex C-MOVE identifiers:
        // public DicomDataset MoveIdentifiers { get; init; }

        /// <summary>
        /// Optional: The Calling AE Title for this SCU operation.
        /// If not provided, a default from DicomGatewaySettings might be used.
        /// </summary>
        public string? CallingAeTitle { get; init; }


        public DicomCMoveRequestDto(DicomAe sourceAe, string destinationAeTitle, List<string> studyInstanceUids, string? callingAeTitle = null)
        {
            if (sourceAe == null)
                throw new ArgumentNullException(nameof(sourceAe));
            if (string.IsNullOrWhiteSpace(destinationAeTitle))
                throw new ArgumentException("Destination AE Title cannot be null or whitespace.", nameof(destinationAeTitle));
            if (studyInstanceUids == null || studyInstanceUids.Count == 0 || studyInstanceUids.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("Study Instance UIDs list cannot be null, empty, or contain invalid UIDs.", nameof(studyInstanceUids));

            SourceAe = sourceAe;
            DestinationAeTitle = destinationAeTitle;
            StudyInstanceUids = studyInstanceUids;
            CallingAeTitle = callingAeTitle;
        }
    }
}