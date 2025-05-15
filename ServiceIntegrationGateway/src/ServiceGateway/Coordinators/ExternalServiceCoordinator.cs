using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Integration.Interfaces;
using TheSSS.DICOMViewer.Integration.Models;
// Assuming ILoggerAdapter is in a cross-cutting assembly, e.g.:
using TheSSS.DICOMViewer.CrossCutting.Abstractions.Logging;
// If specific DTOs for adapter responses are very different, more complex mapping might be needed.
// For now, assuming direct or simple property mapping based on "similar to" descriptions.

namespace TheSSS.DICOMViewer.Integration.Coordinators
{
    /// <summary>
    /// Implements IExternalServiceCoordinator, acting as the central facade
    /// that orchestrates calls to the various specialized service adapters.
    /// </summary>
    public class ExternalServiceCoordinator : IExternalServiceCoordinator
    {
        private readonly IOdooApiAdapter _odooApiAdapter;
        private readonly ISmtpServiceAdapter _smtpServiceAdapter;
        private readonly IWindowsPrintAdapter _windowsPrintAdapter;
        private readonly IDicomNetworkAdapter _dicomNetworkAdapter;
        private readonly IUnifiedErrorHandlingService _unifiedErrorHandlingService;
        private readonly ILoggerAdapter _logger;

        public ExternalServiceCoordinator(
            IOdooApiAdapter odooApiAdapter,
            ISmtpServiceAdapter smtpServiceAdapter,
            IWindowsPrintAdapter windowsPrintAdapter,
            IDicomNetworkAdapter dicomNetworkAdapter,
            IUnifiedErrorHandlingService unifiedErrorHandlingService,
            ILoggerAdapter logger)
        {
            _odooApiAdapter = odooApiAdapter ?? throw new ArgumentNullException(nameof(odooApiAdapter));
            _smtpServiceAdapter = smtpServiceAdapter ?? throw new ArgumentNullException(nameof(smtpServiceAdapter));
            _windowsPrintAdapter = windowsPrintAdapter ?? throw new ArgumentNullException(nameof(windowsPrintAdapter));
            _dicomNetworkAdapter = dicomNetworkAdapter ?? throw new ArgumentNullException(nameof(dicomNetworkAdapter));
            _unifiedErrorHandlingService = unifiedErrorHandlingService ?? throw new ArgumentNullException(nameof(unifiedErrorHandlingService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<GatewayResponse<LicenseValidationResultDto>> ValidateLicenseAsync(string licenseKey, CancellationToken cancellationToken = default)
        {
            _logger.Info($"ServiceCoordinator: Attempting to validate license with key '{licenseKey}'.");
            try
            {
                // The IExternalServiceCoordinator.ValidateLicenseAsync only takes licenseKey.
                // IOdooApiAdapter.ValidateLicenseAsync takes OdooLicenseRequestDto (LicenseKey, DeviceIdentifier).
                // We construct OdooLicenseRequestDto here. DeviceIdentifier must be sourced or be optional.
                // Assuming DeviceIdentifier is optional or handled by the adapter if null.
                var odooAdapterRequest = new OdooLicenseRequestDto(licenseKey, null /* DeviceIdentifier might be resolved by adapter or configuration */);
                
                OdooLicenseResponseDto odooAdapterResponse = await _odooApiAdapter.ValidateLicenseAsync(odooAdapterRequest, cancellationToken);

                // Map OdooLicenseResponseDto to LicenseValidationResultDto
                // This assumes OdooLicenseResponseDto has compatible properties.
                var resultData = new LicenseValidationResultDto(
                    odooAdapterResponse.IsValid, 
                    odooAdapterResponse.ExpiryDate, 
                    odooAdapterResponse.EnabledFeatures ?? new List<string>()
                );

                _logger.Info($"ServiceCoordinator: License validation successful for key '{licenseKey}'.");
                return new GatewayResponse<LicenseValidationResultDto>(true, resultData, null);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"ServiceCoordinator: Error validating license for key '{licenseKey}'.");
                ServiceErrorDto errorDto = _unifiedErrorHandlingService.HandleError(ex, "OdooApiAdapter");
                return new GatewayResponse<LicenseValidationResultDto>(false, null, errorDto);
            }
        }

        public async Task<GatewayResponse<EmailSendResultDto>> SendEmailAsync(EmailDto emailMessage, CancellationToken cancellationToken = default)
        {
            _logger.Info($"ServiceCoordinator: Attempting to send email to '{string.Join(", ", emailMessage.To ?? new List<string>())}' with subject '{emailMessage.Subject}'.");
            try
            {
                SmtpSendResultDto smtpAdapterResponse = await _smtpServiceAdapter.SendEmailAsync(emailMessage, cancellationToken);

                // Map SmtpSendResultDto to EmailSendResultDto.
                // Assuming properties are compatible as per "similar to" description.
                var resultData = new EmailSendResultDto(
                    smtpAdapterResponse.IsSent,
                    smtpAdapterResponse.MessageId,
                    smtpAdapterResponse.StatusMessage
                );

                _logger.Info($"ServiceCoordinator: Email sending process completed for subject '{emailMessage.Subject}'. Success: {resultData.IsSent}.");
                return new GatewayResponse<EmailSendResultDto>(true, resultData, null);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"ServiceCoordinator: Error sending email with subject '{emailMessage.Subject}'.");
                ServiceErrorDto errorDto = _unifiedErrorHandlingService.HandleError(ex, "SmtpServiceAdapter");
                return new GatewayResponse<EmailSendResultDto>(false, null, errorDto);
            }
        }

        public async Task<GatewayResponse<PrintResultDto>> SubmitPrintJobAsync(PrintJobDto printJob, CancellationToken cancellationToken = default)
        {
            _logger.Info($"ServiceCoordinator: Attempting to submit print job for printer '{printJob.PrinterName}'.");
            try
            {
                WindowsPrintResultDto printAdapterResponse = await _windowsPrintAdapter.PrintDocumentAsync(printJob, cancellationToken);

                // Map WindowsPrintResultDto to PrintResultDto.
                // Assuming properties are compatible as per "similar to" description.
                var resultData = new PrintResultDto(
                    printAdapterResponse.IsSubmitted,
                    printAdapterResponse.JobId,
                    printAdapterResponse.StatusMessage
                );

                _logger.Info($"ServiceCoordinator: Print job submission process completed for printer '{printJob.PrinterName}'. Success: {resultData.IsSubmitted}.");
                return new GatewayResponse<PrintResultDto>(true, resultData, null);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"ServiceCoordinator: Error submitting print job for printer '{printJob.PrinterName}'.");
                ServiceErrorDto errorDto = _unifiedErrorHandlingService.HandleError(ex, "WindowsPrintAdapter");
                return new GatewayResponse<PrintResultDto>(false, null, errorDto);
            }
        }

        public async Task<GatewayResponse<DicomOperationResultDto>> ExecuteDicomCStoreAsync(DicomCStoreRequestDto request, CancellationToken cancellationToken = default)
        {
            _logger.Info($"ServiceCoordinator: Attempting DICOM C-STORE to AE '{request.TargetAE}' at '{request.TargetHost}:{request.TargetPort}'.");
            try
            {
                DicomOperationResultDto dicomResult = await _dicomNetworkAdapter.SendCStoreAsync(request, cancellationToken);
                
                _logger.Info($"ServiceCoordinator: DICOM C-STORE operation completed for AE '{request.TargetAE}'. Success: {dicomResult.IsSuccess}, Status: {dicomResult.DicomStatusCode}.");
                return new GatewayResponse<DicomOperationResultDto>(true, dicomResult, null);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"ServiceCoordinator: Error executing DICOM C-STORE to AE '{request.TargetAE}'.");
                ServiceErrorDto errorDto = _unifiedErrorHandlingService.HandleError(ex, "DicomNetworkAdapter");
                return new GatewayResponse<DicomOperationResultDto>(false, null, errorDto);
            }
        }

        public async Task<GatewayResponse<DicomOperationResultDto>> ExecuteDicomCEchoAsync(DicomCEchoRequestDto request, CancellationToken cancellationToken = default)
        {
            _logger.Info($"ServiceCoordinator: Attempting DICOM C-ECHO to AE '{request.TargetAE}' at '{request.TargetHost}:{request.TargetPort}'.");
            try
            {
                DicomOperationResultDto dicomResult = await _dicomNetworkAdapter.SendCEchoAsync(request, cancellationToken);

                _logger.Info($"ServiceCoordinator: DICOM C-ECHO operation completed for AE '{request.TargetAE}'. Success: {dicomResult.IsSuccess}, Status: {dicomResult.DicomStatusCode}.");
                return new GatewayResponse<DicomOperationResultDto>(true, dicomResult, null);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"ServiceCoordinator: Error executing DICOM C-ECHO to AE '{request.TargetAE}'.");
                ServiceErrorDto errorDto = _unifiedErrorHandlingService.HandleError(ex, "DicomNetworkAdapter");
                return new GatewayResponse<DicomOperationResultDto>(false, null, errorDto);
            }
        }

        public async Task<GatewayResponse<DicomCFindResultDto>> ExecuteDicomCFindAsync(DicomCFindRequestDto request, CancellationToken cancellationToken = default)
        {
            _logger.Info($"ServiceCoordinator: Attempting DICOM C-FIND to AE '{request.TargetAE}' at '{request.TargetHost}:{request.TargetPort}' with level '{request.QueryLevel}'.");
            try
            {
                DicomCFindResultDto dicomResult = await _dicomNetworkAdapter.SendCFindAsync(request, cancellationToken);

                _logger.Info($"ServiceCoordinator: DICOM C-FIND operation completed for AE '{request.TargetAE}'. Success: {dicomResult.IsSuccess}, Status: {dicomResult.DicomStatusCode}, Matches: {dicomResult.MatchedDatasets?.Count ?? 0}.");
                return new GatewayResponse<DicomCFindResultDto>(true, dicomResult, null);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"ServiceCoordinator: Error executing DICOM C-FIND to AE '{request.TargetAE}'.");
                ServiceErrorDto errorDto = _unifiedErrorHandlingService.HandleError(ex, "DicomNetworkAdapter");
                return new GatewayResponse<DicomCFindResultDto>(false, null, errorDto);
            }
        }

        public async Task<GatewayResponse<DicomOperationResultDto>> ExecuteDicomCMoveAsync(DicomCMoveRequestDto request, CancellationToken cancellationToken = default)
        {
            _logger.Info($"ServiceCoordinator: Attempting DICOM C-MOVE from AE '{request.TargetAE}' to destination '{request.DestinationAE}'.");
            try
            {
                DicomOperationResultDto dicomResult = await _dicomNetworkAdapter.SendCMoveAsync(request, cancellationToken);

                _logger.Info($"ServiceCoordinator: DICOM C-MOVE operation completed for AE '{request.TargetAE}'. Success: {dicomResult.IsSuccess}, Status: {dicomResult.DicomStatusCode}.");
                return new GatewayResponse<DicomOperationResultDto>(true, dicomResult, null);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, $"ServiceCoordinator: Error executing DICOM C-MOVE from AE '{request.TargetAE}'.");
                ServiceErrorDto errorDto = _unifiedErrorHandlingService.HandleError(ex, "DicomNetworkAdapter");
                return new GatewayResponse<DicomOperationResultDto>(false, null, errorDto);
            }
        }
    }
}