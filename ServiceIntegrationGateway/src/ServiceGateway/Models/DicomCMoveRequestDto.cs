using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Integration.Models;

/// <summary>
/// Data Transfer Object for DICOM C-MOVE requests via the gateway.
/// Specifies the source AE (implicitly the target of the C-MOVE-RQ), the destination AE,
/// and identifiers for the DICOM objects to be moved.
/// </summary>
/// <param name="SourceAe">The DICOM Application Entity from which the data will be moved (i.e., the SCP that processes the C-MOVE RQ).</param>
/// <param name="DestinationAETitle">The Application Entity Title of the destination where the DICOM objects should be sent.</param>
/// <param name="StudyInstanceUids">An optional list of Study Instance UIDs to move. If provided, all series and instances under these studies will be targeted.</param>
/// <param name="SeriesInstanceUids">An optional list of Series Instance UIDs to move. If provided, all instances under these series will be targeted.
/// This is typically used in conjunction with a specific StudyInstanceUid for context.</param>
/// <param name="SopInstanceUids">An optional list of SOP Instance UIDs to move. These are specific image/object instances.</param>
public record DicomCMoveRequestDto(
    DicomAETarget SourceAe, // This is the AE that will perform the C-STORE sub-operations
    string DestinationAETitle,
    List<string>? StudyInstanceUids,
    List<string>? SeriesInstanceUids,
    List<string>? SopInstanceUids
)
{
    /// <summary>
    /// Validates that at least one set of UIDs is provided.
    /// </summary>
    /// <exception cref="ArgumentException">Thrown if no UIDs are specified for the move operation.</exception>
    public void Validate()
    {
        if ((StudyInstanceUids == null || StudyInstanceUids.Count == 0) &&
            (SeriesInstanceUids == null || SeriesInstanceUids.Count == 0) &&
            (SopInstanceUids == null || SopInstanceUids.Count == 0))
        {
            throw new ArgumentException("At least one of StudyInstanceUids, SeriesInstanceUids, or SopInstanceUids must be provided for a C-MOVE operation.");
        }
    }
}