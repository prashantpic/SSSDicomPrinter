using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TheSSS.DICOMViewer.Integration.Configuration;
using TheSSS.DICOMViewer.Integration.Interfaces;
using TheSSS.DICOMViewer.Integration.Models;
using TheSSS.DICOMViewer.CrossCutting.Logging; // Assuming ILoggerAdapter namespace
using TheSSS.DICOMViewer.Infrastructure.DICOM.Client; // Assuming IDicomLowLevelClient namespace
using Polly;

namespace TheSSS.DICOMViewer.Integration.Adapters
{
    public class DicomNetworkAdapter : IDicomNetworkAdapter
    {
        private readonly IDicomLowLevelClient _dicomClient;
        private readonly DicomGatewaySettings _gatewaySettings;
        private readonly IRateLimiter _rateLimiter;
        private readonly IResiliencePolicyProvider _resiliencePolicyProvider;
        private readonly IUnifiedErrorHandlingService _errorHandlingService;
        private readonly ILoggerAdapter<DicomNetworkAdapter> _logger;

        private const string DicomServiceIdentifier = "DICOMNetworkService";

        public DicomNetworkAdapter(
            IDicomLowLevelClient dicomClient,
            IOptions<DicomGatewaySettings> gatewaySettings,
            IRateLimiter rateLimiter,
            IResiliencePolicyProvider resiliencePolicyProvider,
            IUnifiedErrorHandlingService errorHandlingService,
            ILoggerAdapter<DicomNetworkAdapter> logger)
        {
            _dicomClient = dicomClient;
            _gatewaySettings = gatewaySettings.Value;
            _rateLimiter = rateLimiter;
            _resiliencePolicyProvider = resiliencePolicyProvider;
            _errorHandlingService = errorHandlingService;
            _logger = logger;
        }

        private async Task<TResponse> ExecuteDicomOperationAsync<TResponse>(
            string operationName,
            string targetAE,
            Func<CancellationToken, Task<LowLevelDicomResponse>> dicomClientOperation, // Assuming LowLevelDicomResponse from IDicomLowLevelClient
            Func<LowLevelDicomResponse, TResponse> createSuccessResponse,
            CancellationToken cancellationToken)
            where TResponse : DicomOperationResultDto // Base DTO for DICOM results
        {
            _logger.LogInformation("Attempting DICOM {OperationName} to AE: {TargetAE}", operationName, targetAE);

            try
            {
                // Use a resource key specific to the target AE or a general DICOM key
                string rateLimiterKey = $"{PolicyRegistryKeys.DicomNetworkRateLimit}_{targetAE ?? "DefaultAE"}";
                await _rateLimiter.AcquirePermitAsync(rateLimiterKey, cancellationToken);

                var policy = await _resiliencePolicyProvider.GetPolicyAsync(PolicyRegistryKeys.DicomNetworkResiliencePolicy);

                var lowLevelResponse = await policy.ExecuteAsync(token => dicomClientOperation(token), cancellationToken);

                if (!lowLevelResponse.IsSuccess)
                {
                    _logger.LogWarning("DICOM {OperationName} to AE {TargetAE} failed with status: {DicomStatus} - {StatusMessage}",
                        operationName, targetAE, lowLevelResponse.DicomStatus, lowLevelResponse.StatusMessage);
                    var errorDto = _errorHandlingService.HandleErrorResponse(lowLevelResponse, DicomServiceIdentifier); 
                    // Create a failed TResponse; requires TResponse to have a constructor for errors or be mutable.
                    // For simplicity, let's assume DicomOperationResultDto and its derivatives can represent failure.
                    // This might mean TResponse needs a constructor that takes ServiceErrorDto.
                    // Let's assume specific result DTOs have appropriate constructors.
                    // Here, we throw a custom exception that ExternalServiceCoordinator will handle.
                    throw new DicomNetworkOperationException(errorDto.Message, errorDto, lowLevelResponse.DicomStatus);
                }

                _logger.LogInformation("DICOM {OperationName} to AE {TargetAE} successful.", operationName, targetAE);
                return createSuccessResponse(lowLevelResponse);
            }
            catch (DicomNetworkOperationException) // Already handled
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during DICOM {OperationName} to AE {TargetAE}", operationName, targetAE);
                var errorDto = _errorHandlingService.HandleError(ex, DicomServiceIdentifier);
                throw new DicomNetworkOperationException(errorDto.Message, ex, errorDto, 0x0000); // Generic failure status
            }
        }

        public async Task<DicomOperationResultDto> SendCStoreAsync(DicomCStoreRequestDto request, CancellationToken cancellationToken)
        {
            return await ExecuteDicomOperationAsync(
                "C-STORE",
                request.TargetAE,
                token => _dicomClient.SendCStoreAsync(
                    request.TargetAE, request.TargetHost, request.TargetPort,
                    request.DicomFilePaths, request.DicomFileStreams, token), // Assuming IDicomLowLevelClient signature matches
                lowLevelResp => new DicomOperationResultDto(
                    true, lowLevelResp.DicomStatus, lowLevelResp.StatusMessage, lowLevelResp.AffectedSopInstanceUids ?? new List<string>()),
                cancellationToken);
        }

        public async Task<DicomOperationResultDto> SendCEchoAsync(DicomCEchoRequestDto request, CancellationToken cancellationToken)
        {
            return await ExecuteDicomOperationAsync(
                "C-ECHO",
                request.TargetAE,
                token => _dicomClient.SendCEchoAsync(request.TargetAE, request.TargetHost, request.TargetPort, token),
                lowLevelResp => new DicomOperationResultDto(true, lowLevelResp.DicomStatus, lowLevelResp.StatusMessage, new List<string>()),
                cancellationToken);
        }

        public async Task<DicomCFindResultDto> SendCFindAsync(DicomCFindRequestDto request, CancellationToken cancellationToken)
        {
             return await ExecuteDicomOperationAsync(
                "C-FIND",
                request.TargetAE,
                token => _dicomClient.SendCFindAsync(
                    request.TargetAE, request.TargetHost, request.TargetPort,
                    request.QueryLevel, request.QueryDataset, token), // QueryDataset might be FO-DICOM DicomDataset or similar
                lowLevelResp => new DicomCFindResultDto(
                    true, lowLevelResp.DicomStatus, lowLevelResp.StatusMessage, lowLevelResp.FoundDatasets ?? new List<object>()),
                cancellationToken);
        }

        public async Task<DicomOperationResultDto> SendCMoveAsync(DicomCMoveRequestDto request, CancellationToken cancellationToken)
        {
            return await ExecuteDicomOperationAsync(
                "C-MOVE",
                request.TargetAE,
                token => _dicomClient.SendCMoveAsync(
                    request.TargetAE, request.TargetHost, request.TargetPort,
                    request.DestinationAE, request.IdentifiersDataset, token),
                lowLevelResp => new DicomOperationResultDto(
                    true, lowLevelResp.DicomStatus, lowLevelResp.StatusMessage, lowLevelResp.AffectedSopInstanceUids ?? new List<string>()),
                cancellationToken);
        }
    }

    // Custom exception for DICOM Network operations
    public class DicomNetworkOperationException : Exception
    {
        public ServiceErrorDto ErrorDetails { get; }
        public ushort DicomStatus { get; }

        public DicomNetworkOperationException(string message, ServiceErrorDto errorDetails, ushort dicomStatus) : base(message)
        {
            ErrorDetails = errorDetails;
            DicomStatus = dicomStatus;
        }
        public DicomNetworkOperationException(string message, Exception innerException, ServiceErrorDto errorDetails, ushort dicomStatus) : base(message, innerException)
        {
            ErrorDetails = errorDetails;
            DicomStatus = dicomStatus;
        }
    }
}