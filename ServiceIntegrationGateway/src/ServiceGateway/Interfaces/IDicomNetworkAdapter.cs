using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Integration.Models;

namespace TheSSS.DICOMViewer.Integration.Interfaces;

/// <summary>
/// Defines the contract for executing DICOM C-STORE, C-ECHO, C-FIND, and C-MOVE operations,
/// applying resilience policies and handling DICOM-specific errors.
/// </summary>
public interface IDicomNetworkAdapter
{
    /// <summary>
    /// Sends DICOM instances using C-STORE.
    /// </summary>
    /// <param name="request">The C-STORE request details.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, with the DICOM operation result.</returns>
    Task<DicomOperationResultDto> SendCStoreAsync(DicomCStoreRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a DICOM C-ECHO verification.
    /// </summary>
    /// <param name="request">The C-ECHO request details.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, with the DICOM operation result.</returns>
    Task<DicomOperationResultDto> SendCEchoAsync(DicomCEchoRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Queries a DICOM entity using C-FIND.
    /// </summary>
    /// <param name="request">The C-FIND request details.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, with the DICOM C-FIND result.</returns>
    Task<DicomCFindResultDto> SendCFindAsync(DicomCFindRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Moves DICOM instances using C-MOVE.
    /// </summary>
    /// <param name="request">The C-MOVE request details.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, with the DICOM operation result.</returns>
    Task<DicomOperationResultDto> SendCMoveAsync(DicomCMoveRequestDto request, CancellationToken cancellationToken = default);
}