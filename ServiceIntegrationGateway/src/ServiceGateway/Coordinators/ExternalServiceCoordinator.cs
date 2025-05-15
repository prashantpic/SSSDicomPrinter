using System;
using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Common.Logging; // Assuming ILoggerAdapter is here
using TheSSS.DICOMViewer.Integration.Interfaces;
using TheSSS.DICOMViewer.Integration.Models;

namespace TheSSS.DICOMViewer.Integration.Coordinators
{
    /// <summary>
    /// Implements IExternalServiceCoordinator, acting as the central facade (ApiCoordinator)
    /// that orchestrates calls to the various specialized service adapters (Odoo, SMTP, Print, DICOM).
    /// </summary>
    public class ExternalServiceCoordinator : IExternalServiceCoordinator
    {
        private readonly IOdooApiAdapter _odooAdapter;
        private readonly ISmtpServiceAdapter _smtpAdapter;
        private readonly IWindowsPrintAdapter _printAdapter;
        private readonly IDicomNetworkAdapter _dicomAdapter;
        private readonly IUnifiedErrorHandlingService _errorHandler;
        private readonly ILoggerAdapter<ExternalServiceCoordinator> _logger;

        public ExternalServiceCoordinator(
            IOdooApiAdapter odooAdapter,
            ISmtpServiceAdapter smtpAdapter,
            IWindowsPrintAdapter printAdapter,
            IDicomNetworkAdapter dicomAdapter,
            IUnifiedErrorHandlingService errorHandler,
            ILoggerAdapter<ExternalServiceCoordinator> logger)
        {
            _odooAdapter = odooAdapter ?? throw new ArgumentNullException(nameof(odooAdapter));
            _smtpAdapter = smtpAdapter ?? throw new ArgumentNullException(nameof(smtpAdapter));
            _printAdapter = printAdapter ?? throw new ArgumentNullException(nameof(printAdapter));
            _dicomAdapter = dicomAdapter ?? throw new ArgumentNullException(nameof(dicomAdapter));
            _errorHandler = errorHandler ?? throw new ArgumentNullException(nameof(errorHandler));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public async Task<GatewayResponse<LicenseValidationResultDto>> ValidateLicenseAsync(string licenseKey, CancellationToken cancellationToken = default)
        {
            _logger.Debug("ExternalServiceCoordinator: Validating license via Odoo adapter for key: {LicenseKeyFirstChars}...", licenseKey?.Substring(0, Math.Min(licenseKey.Length, 4)));
            try
            {
                var odooRequest = new OdooLicenseRequestDto(licenseKey);
                var odooResponse = await _odooAdapter.ValidateLicenseAsync(odooRequest, cancellationToken);

                // Map Odoo-specific response DTO to generic Gateway DTO
                var result = new LicenseValidationResultDto(
                    isValid: odooResponse.IsValid, 
                    expiryDate: odooResponse.ExpiryDate, 
                    features: odooResponse.Features, 
                    errorMessage: odooResponse.ErrorMessage 
                );

                // Check for domain-level errors indicated in the Odoo response itself (not exceptions)
                if (!result.IsValid && !string.IsNullOrEmpty(result.ErrorMessage))
                {
                    // Use error handler for domain errors included in the response body
                    var serviceError = _errorHandler.HandleErrorResponse(odooResponse, "Odoo");
                     _logger.Warning($"Odoo license validation returned domain error: {serviceError.Message}");
                    return GatewayResponse<LicenseValidationResultDto>.Failure(serviceError);
                }

                _logger.Info("Odoo license validation successful for key: {LicenseKeyFirstChars}..., IsValid: {IsValid}", licenseKey?.Substring(0, Math.Min(licenseKey.Length, 4)), result.IsValid);
                return GatewayResponse<LicenseValidationResultDto>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during Odoo license validation for key: {LicenseKeyFirstChars}...", licenseKey?.Substring(0, Math.Min(licenseKey.Length, 4)));
                var serviceError = _errorHandler.HandleError(ex, "Odoo");
                return GatewayResponse<LicenseValidationResultDto>.Failure(serviceError);
            }
        }

        /// <inheritdoc />
        public async Task<GatewayResponse<EmailSendResultDto>> SendEmailAsync(EmailDto emailMessage, CancellationToken cancellationToken = default)
        {
            _logger.Debug("ExternalServiceCoordinator: Sending email via SMTP adapter to {ToAddresses}.", string.Join(",", emailMessage.ToAddresses));
            try
            {
                var smtpResult = await _smtpAdapter.SendEmailAsync(emailMessage, cancellationToken);

                // Map SMTP-specific result DTO to generic Gateway DTO
                var result = new EmailSendResultDto(
                    isSent: smtpResult.IsSuccess, 
                    messageId: smtpResult.MessageId, 
                    statusMessage: smtpResult.StatusMessage 
                );

                 if (!result.IsSent)
                 {
                     var serviceError = _errorHandler.HandleErrorResponse(smtpResult, "SMTP");
                     _logger.Warning($"SMTP adapter reported failure: {result.StatusMessage}");
                     return GatewayResponse<EmailSendResultDto>.Failure(serviceError);
                 }

                _logger.Info("Email sent successfully via SMTP adapter to {ToAddresses}, MessageId: {MessageId}.", string.Join(",", emailMessage.ToAddresses), result.MessageId);
                return GatewayResponse<EmailSendResultDto>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during email sending via SMTP adapter to {ToAddresses}.", string.Join(",", emailMessage.ToAddresses));
                var serviceError = _errorHandler.HandleError(ex, "SMTP");
                return GatewayResponse<EmailSendResultDto>.Failure(serviceError);
            }
        }

        /// <inheritdoc />
        public async Task<GatewayResponse<PrintResultDto>> SubmitPrintJobAsync(PrintJobDto printJob, CancellationToken cancellationToken = default)
        {
            _logger.Debug("ExternalServiceCoordinator: Submitting print job '{JobTitle}' via Windows Print adapter to printer '{PrinterName}'.", printJob.JobTitle, printJob.TargetPrinterName);
            try
            {
                var printResultAdapter = await _printAdapter.PrintDocumentAsync(printJob, cancellationToken);

                // Map Windows Print-specific result DTO to generic Gateway DTO
                var result = new PrintResultDto(
                    isSubmitted: printResultAdapter.IsSuccess, 
                    jobId: printResultAdapter.JobId, 
                    statusMessage: printResultAdapter.StatusMessage
                );

                 if (!result.IsSubmitted)
                 {
                     var serviceError = _errorHandler.HandleErrorResponse(printResultAdapter, "Print");
                     _logger.Warning($"Windows Print adapter reported failure: {result.StatusMessage}");
                     return GatewayResponse<PrintResultDto>.Failure(serviceError);
                 }

                _logger.Info("Print job '{JobTitle}' submitted successfully. JobId: {JobId}.", printJob.JobTitle, result.JobId);
                return GatewayResponse<PrintResultDto>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during print job '{JobTitle}' submission.", printJob.JobTitle);
                var serviceError = _errorHandler.HandleError(ex, "Print");
                return GatewayResponse<PrintResultDto>.Failure(serviceError);
            }
        }

        /// <inheritdoc />
        public async Task<GatewayResponse<DicomOperationResultDto>> ExecuteDicomCStoreAsync(DicomCStoreRequestDto request, CancellationToken cancellationToken = default)
        {
            _logger.Debug("ExternalServiceCoordinator: Executing DICOM C-STORE via adapter to AE '{TargetAETitle}'. Files: {FileCount}", request.TargetAE.AeTitle, request.DicomFilePaths.Count);
            try
            {
                var dicomResult = await _dicomAdapter.SendCStoreAsync(request, cancellationToken);

                if (!dicomResult.IsSuccess)
                 {
                      var serviceError = _errorHandler.HandleErrorResponse(dicomResult, "DICOM");
                      _logger.Warning($"DICOM C-STORE adapter reported failure: {dicomResult.StatusMessage}. Status Code: {dicomResult.DicomStatusCode}");
                     return GatewayResponse<DicomOperationResultDto>.Failure(serviceError);
                 }

                _logger.Info("DICOM C-STORE operation successful to AE '{TargetAETitle}'. Affected SOP Instance UID: {SopInstanceUid}", request.TargetAE.AeTitle, dicomResult.AffectedSopInstanceUid);
                return GatewayResponse<DicomOperationResultDto>.Success(dicomResult);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during DICOM C-STORE operation to AE '{TargetAETitle}'.", request.TargetAE.AeTitle);
                var serviceError = _errorHandler.HandleError(ex, "DICOM");
                return GatewayResponse<DicomOperationResultDto>.Failure(serviceError);
            }
        }

        /// <inheritdoc />
        public async Task<GatewayResponse<DicomOperationResultDto>> ExecuteDicomCEchoAsync(DicomCEchoRequestDto request, CancellationToken cancellationToken = default)
        {
            _logger.Debug("ExternalServiceCoordinator: Executing DICOM C-ECHO via adapter to AE '{TargetAETitle}'.", request.TargetAE.AeTitle);
             try
            {
                var dicomResult = await _dicomAdapter.SendCEchoAsync(request, cancellationToken);

                 if (!dicomResult.IsSuccess)
                 {
                      var serviceError = _errorHandler.HandleErrorResponse(dicomResult, "DICOM");
                      _logger.Warning($"DICOM C-ECHO adapter reported failure: {dicomResult.StatusMessage}. Status Code: {dicomResult.DicomStatusCode}");
                     return GatewayResponse<DicomOperationResultDto>.Failure(serviceError);
                 }

                _logger.Info("DICOM C-ECHO operation successful for AE '{TargetAETitle}'.", request.TargetAE.AeTitle);
                return GatewayResponse<DicomOperationResultDto>.Success(dicomResult);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during DICOM C-ECHO operation to AE '{TargetAETitle}'.", request.TargetAE.AeTitle);
                var serviceError = _errorHandler.HandleError(ex, "DICOM");
                return GatewayResponse<DicomOperationResultDto>.Failure(serviceError);
            }
        }

        /// <inheritdoc />
        public async Task<GatewayResponse<DicomCFindResultDto>> ExecuteDicomCFindAsync(DicomCFindRequestDto request, CancellationToken cancellationToken = default)
        {
            _logger.Debug("ExternalServiceCoordinator: Executing DICOM C-FIND via adapter to AE '{TargetAETitle}' at level '{QueryLevel}'.", request.TargetAE.AeTitle, request.QueryLevel);
             try
            {
                var dicomResult = await _dicomAdapter.SendCFindAsync(request, cancellationToken);

                 if (!dicomResult.IsSuccess)
                 {
                      var serviceError = _errorHandler.HandleErrorResponse(dicomResult, "DICOM");
                      _logger.Warning($"DICOM C-FIND adapter reported failure: {dicomResult.StatusMessage}. Status Code: {dicomResult.DicomStatusCode}");
                     return GatewayResponse<DicomCFindResultDto>.Failure(serviceError);
                 }

                _logger.Info("DICOM C-FIND operation successful for AE '{TargetAETitle}'. Found {MatchCount} matches.", request.TargetAE.AeTitle, dicomResult.MatchedDatasets?.Count ?? 0);
                return GatewayResponse<DicomCFindResultDto>.Success(dicomResult);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during DICOM C-FIND operation to AE '{TargetAETitle}'.", request.TargetAE.AeTitle);
                var serviceError = _errorHandler.HandleError(ex, "DICOM");
                return GatewayResponse<DicomCFindResultDto>.Failure(serviceError);
            }
        }

        /// <inheritdoc />
        public async Task<GatewayResponse<DicomOperationResultDto>> ExecuteDicomCMoveAsync(DicomCMoveRequestDto request, CancellationToken cancellationToken = default)
        {
            _logger.Debug("ExternalServiceCoordinator: Executing DICOM C-MOVE via adapter from AE '{SourceAETitle}' to '{DestinationAETitle}'.", request.TargetAE.AeTitle, request.DestinationAE);
             try
            {
                var dicomResult = await _dicomAdapter.SendCMoveAsync(request, cancellationToken);

                 if (!dicomResult.IsSuccess)
                 {
                      var serviceError = _errorHandler.HandleErrorResponse(dicomResult, "DICOM");
                      _logger.Warning($"DICOM C-MOVE adapter reported failure: {dicomResult.StatusMessage}. Status Code: {dicomResult.DicomStatusCode}");
                     return GatewayResponse<DicomOperationResultDto>.Failure(serviceError);
                 }

                _logger.Info("DICOM C-MOVE operation successful from AE '{SourceAETitle}' to '{DestinationAETitle}'.", request.TargetAE.AeTitle, request.DestinationAE);
                return GatewayResponse<DicomOperationResultDto>.Success(dicomResult);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during DICOM C-MOVE operation from AE '{SourceAETitle}' to '{DestinationAETitle}'.", request.TargetAE.AeTitle, request.DestinationAE);
                var serviceError = _errorHandler.HandleError(ex, "DICOM");
                return GatewayResponse<DicomOperationResultDto>.Failure(serviceError);
            }
        }
    }
}