using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Integration.Models;

namespace TheSSS.DICOMViewer.Integration.Interfaces;

/// <summary>
/// Defines the contract for executing DICOM C-STORE, C-ECHO, C-FIND, and C-MOVE operations, 
/// applying resilience policies and handling DICOM-specific errors. This interface is for adapters 
/// handling DICOM network communications, wrapping a core DICOM client with added gateway logic.
/// </summary>
public interface IDicomNetworkAdapter
{
    /// <summary>
    /// Executes a DICOM C-STORE operation.
    /// </summary>
    /// <param name="request">The C-STORE request details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the C-STORE operation.</returns>
    Task<DicomOperationResultDto> SendCStoreAsync(DicomCStoreRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a DICOM C-ECHO operation.
    /// </summary>
    /// <param name="request">The C-ECHO request details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the C-ECHO operation.</returns>
    Task<DicomOperationResultDto> SendCEchoAsync(DicomCEchoRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a DICOM C-FIND operation.
    /// </summary>
    /// <param name="request">The C-FIND request details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the C-FIND operation, including matched datasets.</returns>
    Task<DicomCFindResultDto> SendCFindAsync(DicomCFindRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a DICOM C-MOVE operation.
    /// </summary>
    /// <param name="request">The C-MOVE request details.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the C-MOVE operation.</returns>
    Task<DicomOperationResultDto> SendCMoveAsync(DicomCMoveRequestDto request, CancellationToken cancellationToken = default);
}