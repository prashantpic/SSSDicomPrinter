using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using RestSharp;
using Polly;
using TheSSS.DICOMViewer.Integration.Interfaces;
using TheSSS.DICOMViewer.Integration.Models;
using TheSSS.DICOMViewer.Integration.Configuration;
using TheSSS.DICOMViewer.Common.Interfaces; // Assuming ILoggerAdapter is here

namespace TheSSS.DICOMViewer.Integration.Adapters;

public class OdooApiAdapter : IOdooApiAdapter
{
    private readonly RestClient _restClient;
    private readonly OdooApiSettings _settings;
    private readonly IResiliencePolicyProvider _policyProvider;
    private readonly IRateLimiter _rateLimiter;
    private readonly ICredentialManager _credentialManager;
    private readonly IUnifiedErrorHandlingService _errorHandlingService;
    private readonly ILoggerAdapter _logger;
    private readonly IAsyncPolicy _resiliencePolicy;
    private readonly ServiceGatewaySettings _gatewaySettings; // To check if Odoo is enabled

    public OdooApiAdapter(
        IOptions<OdooApiSettings> settings,
        IOptions<ServiceGatewaySettings> gatewaySettings,
        IResiliencePolicyProvider policyProvider,
        IRateLimiter rateLimiter,
        ICredentialManager credentialManager,
        IUnifiedErrorHandlingService errorHandlingService,
        ILoggerAdapter logger)
    {
        _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
        _gatewaySettings = gatewaySettings.Value ?? throw new ArgumentNullException(nameof(gatewaySettings));
        _policyProvider = policyProvider ?? throw new ArgumentNullException(nameof(policyProvider));
        _rateLimiter = rateLimiter ?? throw new ArgumentNullException(nameof(rateLimiter));
        _credentialManager = credentialManager ?? throw new ArgumentNullException(nameof(credentialManager));
        _errorHandlingService = errorHandlingService ?? throw new ArgumentNullException(nameof(errorHandlingService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (string.IsNullOrWhiteSpace(_settings.BaseUrl))
        {
            _logger.Warning("Odoo API BaseUrl is not configured.");
            // Initialize with a dummy client or throw if integration is enabled
            _restClient = new RestClient(); // Will fail on request if base URL is empty
        }
        else
        {
            _restClient = new RestClient(_settings.BaseUrl);
        }

        // Get the configured resilience policy for Odoo API calls
        _resiliencePolicy = _policyProvider.GetPolicyAsync(_settings.PolicyKey);
    }

    public async Task<OdooLicenseResponseDto> ValidateLicenseAsync(OdooLicenseRequestDto request, CancellationToken cancellationToken = default)
    {
        if (!_gatewaySettings.EnableOdooIntegration)
        {
            _logger.Information("Odoo API integration is disabled. Skipping license validation.");
            // Return a predefined 'disabled' or 'failed' response suitable for the application
            // Or throw a specific exception indicating the feature is disabled.
             throw new ServiceIntegrationDisabledException("Odoo API integration is disabled in settings.");
        }

        if (string.IsNullOrWhiteSpace(_settings.BaseUrl))
        {
             _logger.Error("Odoo API BaseUrl is not configured. Cannot perform license validation.");
             throw new InvalidOperationException("Odoo API BaseUrl is not configured.");
        }

        // Acquire rate limit permit before executing the operation
        if (_gatewaySettings.RateLimiting.EnableRateLimitingPerService)
        {
             await _rateLimiter.AcquirePermitAsync(_settings.RateLimitResourceKey, cancellationToken);
        }

        ServiceCredentials credentials = await _credentialManager.GetCredentialsAsync(_settings.ServiceIdentifierForCredentials, cancellationToken);

        var apiEndpoint = string.Format(_settings.LicenseValidationEndpoint, _settings.ApiVersion);
        var restRequest = new RestRequest(apiEndpoint, Method.Post);

        // Add credentials (assuming basic auth, API key, or similar based on ServiceCredentials)
        // This part needs to be adapted based on the actual Odoo API authentication method
        if (!string.IsNullOrWhiteSpace(credentials.ApiKey)) // Assuming API key is primary for Odoo
        {
             restRequest.AddHeader("X-Api-Key", credentials.ApiKey); // Example header for API Key
        }
        else if (!string.IsNullOrWhiteSpace(credentials.Username) && !string.IsNullOrWhiteSpace(credentials.Password))
        {
            restRequest.AddHeader("Authorization", $"Basic {Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{credentials.Username}:{credentials.Password}"))}");
        }
        else if (!string.IsNullOrWhiteSpace(credentials.Token)) // Bearer token
        {
            restRequest.AddHeader("Authorization", $"Bearer {credentials.Token}");
        }
        else
        {
            _logger.Warning($"No suitable credentials found for Odoo API ({_settings.ServiceIdentifierForCredentials}). Request may fail if authentication is required.");
        }
        
        // Add request body
        restRequest.AddJsonBody(request);

        // Execute the request with resilience policy
        var response = await _resiliencePolicy.ExecuteAsync(
            () => _restClient.ExecuteAsync<OdooLicenseResponseDto>(restRequest, cancellationToken)
        );

        if (response.IsSuccessful && response.Data != null)
        {
            _logger.Information($"Odoo API license validation successful for key: {request.LicenseKey}.");
            return response.Data;
        }
        else
        {
            // Log specific Odoo API error response if available, otherwise log generic error
            var errorMsg = response.ErrorMessage ?? response.Content ?? "Unknown Odoo API error.";
            _logger.Error($"Odoo API call failed. Status: {response.StatusCode}, Error: {errorMsg}");

            object? serviceSpecificErrorDetails = null;
            if (!string.IsNullOrWhiteSpace(response.Content))
            {
                 try
                 {
                     // Example: If Odoo returns errors in the same DTO structure or a different one
                     // For simplicity, assume errors might be in the OdooLicenseResponseDto itself or a generic structure
                     serviceSpecificErrorDetails = response.Data; // If error details are in the same DTO
                 }
                 catch (Exception jsonEx)
                 {
                     _logger.Error(jsonEx, "Failed to parse Odoo API error response content.");
                 }
            }

            // Throw a custom exception that can be handled by the error handling service
            throw new OdooApiException($"Odoo API request failed: {response.StatusCode}", response.StatusCode, response.Content, serviceSpecificErrorDetails);
        }
    }
}

// Custom exception for Odoo API errors
public class OdooApiException : Exception
{
    public System.Net.HttpStatusCode? StatusCode { get; }
    public string? ResponseContent { get; }
    public object? ServiceSpecificErrorDetails { get; } // If Odoo has a structured error response

    public OdooApiException(string message, System.Net.HttpStatusCode? statusCode, string? responseContent, object? serviceSpecificErrorDetails)
        : base(message)
    {
        StatusCode = statusCode;
        ResponseContent = responseContent;
        ServiceSpecificErrorDetails = serviceSpecificErrorDetails;
    }

    public OdooApiException(string message, Exception innerException, System.Net.HttpStatusCode? statusCode, string? responseContent, object? serviceSpecificErrorDetails)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ResponseContent = responseContent;
        ServiceSpecificErrorDetails = serviceSpecificErrorDetails;
    }
}

// Custom exception for when a service integration is disabled
public class ServiceIntegrationDisabledException : InvalidOperationException
{
    public ServiceIntegrationDisabledException(string message) : base(message) { }
}