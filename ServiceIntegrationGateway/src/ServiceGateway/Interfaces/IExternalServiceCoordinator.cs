using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Integration.Models;

namespace TheSSS.DICOMViewer.Integration.Interfaces;

/// <summary>
/// Defines the contract for the ApiCoordinator component. It provides a single, 
/// simplified interface to external services, abstracting the complexities of 
/// individual adapters, resilience, and error handling.
/// </summary>
public interface IExternalServiceCoordinator
{
    /// <summary>
    /// Validates a license key using the Odoo API.
    /// </summary>
    /// <param name="licenseKey">The license key to validate.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, with a gateway response containing the license validation result.</returns>
    Task<GatewayResponse<LicenseValidationResultDto>> ValidateLicenseAsync(string licenseKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an email message using the configured SMTP service.
    /// </summary>
    /// <param name="emailMessage">The email message to send.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, with a gateway response containing the email send result.</returns>
    Task<GatewayResponse<EmailSendResultDto>> SendEmailAsync(EmailDto emailMessage, CancellationToken cancellationToken = default);

    /// <summary>
    /// Submits a print job to the Windows Print API.
    /// </summary>
    /// <param name="printJob">The print job details.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, with a gateway response containing the print result.</returns>
    Task<GatewayResponse<PrintResultDto>> SubmitPrintJobAsync(PrintJobDto printJob, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a DICOM C-STORE operation.
    /// </summary>
    /// <param name="request">The C-STORE request details.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, with a gateway response containing the DICOM operation result.</returns>
    Task<GatewayResponse<DicomOperationResultDto>> ExecuteDicomCStoreAsync(DicomCStoreRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a DICOM C-ECHO operation.
    /// </summary>
    /// <param name="request">The C-ECHO request details.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, with a gateway response containing the DICOM operation result.</returns>
    Task<GatewayResponse<DicomOperationResultDto>> ExecuteDicomCEchoAsync(DicomCEchoRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a DICOM C-FIND operation.
    /// </summary>
    /// <param name="request">The C-FIND request details.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, with a gateway response containing the DICOM C-FIND result.</returns>
    Task<GatewayResponse<DicomCFindResultDto>> ExecuteDicomCFindAsync(DicomCFindRequestDto request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Executes a DICOM C-MOVE operation.
    /// </summary>
    /// <param name="request">The C-MOVE request details.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A task representing the asynchronous operation, with a gateway response containing the DICOM operation result.</returns>
    Task<GatewayResponse<DicomOperationResultDto>> ExecuteDicomCMoveAsync(DicomCMoveRequestDto request, CancellationToken cancellationToken = default);
}