using System;
using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Integration.Models; // Assuming DTOs are in this namespace

namespace TheSSS.DICOMViewer.Integration.Interfaces
{
    /// <summary>
    /// Interface for adapters handling DICOM network communications (C-STORE, C-ECHO, C-FIND, C-MOVE),
    /// wrapping a core DICOM client with added gateway logic like resilience.
    /// </summary>
    public interface IDicomNetworkAdapter
    {
        /// <summary>
        /// Sends a DICOM C-STORE request.
        /// </summary>
        /// <param name="request">The C-STORE request details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The result of the C-STORE operation.</returns>
        Task<DicomOperationResultDto> SendCStoreAsync(DicomCStoreRequestDto request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a DICOM C-ECHO request.
        /// </summary>
        /// <param name="request">The C-ECHO request details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The result of the C-ECHO operation.</returns>
        Task<DicomOperationResultDto> SendCEchoAsync(DicomCEchoRequestDto request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a DICOM C-FIND request.
        /// </summary>
        /// <param name="request">The C-FIND request details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The result of the C-FIND operation, including found datasets.</returns>
        Task<DicomCFindResultDto> SendCFindAsync(DicomCFindRequestDto request, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends a DICOM C-MOVE request.
        /// </summary>
        /// <param name="request">The C-MOVE request details.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The result of the C-MOVE operation.</returns>
        Task<DicomOperationResultDto> SendCMoveAsync(DicomCMoveRequestDto request, CancellationToken cancellationToken = default);
    }
}