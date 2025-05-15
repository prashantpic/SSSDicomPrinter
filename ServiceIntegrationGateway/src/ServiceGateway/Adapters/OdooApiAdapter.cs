using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RestSharp;
using TheSSS.DICOMViewer.Integration.Configuration;
using TheSSS.DICOMViewer.Integration.Interfaces;
using TheSSS.DICOMViewer.Integration.Models;
using TheSSS.DICOMViewer.CrossCutting.Logging; // Assuming this is the namespace for ILoggerAdapter
using Polly;

namespace TheSSS.DICOMViewer.Integration.Adapters
{
    public class OdooApiAdapter : IOdooApiAdapter
    {
        private readonly RestClient _restClient;
        private readonly OdooApiSettings _odooApiSettings;
        private readonly ICredentialManager _credentialManager;
        private readonly IRateLimiter _rateLimiter;
        private readonly IResiliencePolicyProvider _resiliencePolicyProvider;
        private readonly IUnifiedErrorHandlingService _errorHandlingService;
        private readonly ILoggerAdapter<OdooApiAdapter> _logger;

        private const string OdooServiceIdentifier = "OdooLicensingAPI"; // Or from config

        public OdooApiAdapter(
            IOptions<OdooApiSettings> odooApiSettings,
            ICredentialManager credentialManager,
            IRateLimiter rateLimiter,
            IResiliencePolicyProvider resiliencePolicyProvider,
            IUnifiedErrorHandlingService errorHandlingService,
            ILoggerAdapter<OdooApiAdapter> logger,
            HttpClient httpClient) // RestClient can take HttpClient
        {
            _odooApiSettings = odooApiSettings.Value;
            _credentialManager = credentialManager;
            _rateLimiter = rateLimiter;
            _resiliencePolicyProvider = resiliencePolicyProvider;
            _errorHandlingService = errorHandlingService;
            _logger = logger;

            var options = new RestClientOptions(_odooApiSettings.BaseUrl)
            {
                // Configure other RestClientOptions if needed
            };
            _restClient = new RestClient(httpClient, options);
        }

        public async Task<OdooLicenseResponseDto> ValidateLicenseAsync(OdooLicenseRequestDto request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to validate license for key: {LicenseKey}", request.LicenseKey);

            try
            {
                await _rateLimiter.AcquirePermitAsync(PolicyRegistryKeys.OdooApiRateLimit, cancellationToken);

                var policy = await _resiliencePolicyProvider.GetPolicyAsync(PolicyRegistryKeys.DefaultApiResiliencePolicy); // Or a specific Odoo policy key

                return await policy.ExecuteAsync(async token =>
                {
                    var serviceCredentials = await _credentialManager.GetCredentialsAsync(_odooApiSettings.ServiceIdentifier ?? OdooServiceIdentifier, token);
                    
                    var restRequest = new RestRequest(_odooApiSettings.ValidateEndpoint, Method.Post);
                    restRequest.AddHeader("Authorization", $"Bearer {serviceCredentials.ApiKey}"); // Assuming API Key auth
                    restRequest.AddJsonBody(request);

                    _logger.LogInformation("Sending license validation request to Odoo API.");
                    var response = await _restClient.ExecuteAsync<OdooLicenseResponseDto>(restRequest, token);

                    if (!response.IsSuccessful)
                    {
                        _logger.LogError(response.ErrorException, "Odoo API request failed. Status: {StatusCode}, Content: {Content}", response.StatusCode, response.Content);
                        // Assuming OdooLicenseResponseDto can also represent an error structure from Odoo.
                        // If Odoo returns a specific error DTO, parse that. Otherwise, use HandleErrorResponse with raw content.
                        var errorDto = _errorHandlingService.HandleErrorResponse(response.Content ?? response.ErrorMessage, OdooServiceIdentifier);
                        // This adapter should return the DTO or throw an exception that the coordinator catches.
                        // For now, let's assume OdooLicenseResponseDto itself might contain error flags or we throw a custom exception.
                        // If response.Data is null and it's an error, OdooLicenseResponseDto should reflect that or this should throw.
                        // Let's throw a specific exception that ExternalServiceCoordinator can catch and convert using UnifiedErrorHandlingService.
                        throw new OdooApiException(errorDto.Message, response.ErrorException, errorDto);
                    }
                    
                    if (response.Data == null)
                    {
                         _logger.LogError(response.ErrorException, "Odoo API request was successful but returned no data. Status: {StatusCode}, Content: {Content}", response.StatusCode, response.Content);
                         var errorDto = _errorHandlingService.HandleErrorResponse(response.Content ?? "No data returned", OdooServiceIdentifier);
                         throw new OdooApiException(errorDto.Message, response.ErrorException, errorDto);
                    }

                    _logger.LogInformation("License validation successful for key: {LicenseKey}", request.LicenseKey);
                    return response.Data;

                }, cancellationToken);
            }
            catch (OdooApiException) // Already handled by UnifiedErrorHandlingService
            {
                throw; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while validating license with Odoo API for key: {LicenseKey}", request.LicenseKey);
                var errorDto = _errorHandlingService.HandleError(ex, OdooServiceIdentifier);
                // Throw a custom exception that ExternalServiceCoordinator can catch.
                throw new OdooApiException(errorDto.Message, ex, errorDto);
            }
        }
    }

    // Custom exception for Odoo API interactions
    public class OdooApiException : Exception
    {
        public ServiceErrorDto ErrorDetails { get; }

        public OdooApiException(string message, ServiceErrorDto errorDetails) : base(message)
        {
            ErrorDetails = errorDetails;
        }

        public OdooApiException(string message, Exception innerException, ServiceErrorDto errorDetails) : base(message, innerException)
        {
            ErrorDetails = errorDetails;
        }
    }
}