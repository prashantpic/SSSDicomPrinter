using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TheSSS.DICOMViewer.Integration.Configuration;
using TheSSS.DICOMViewer.Integration.Interfaces;
using TheSSS.DICOMViewer.Integration.Models;
using TheSSS.DICOMViewer.CrossCutting.Logging; // Assuming ILoggerAdapter namespace
// using TheSSS.DICOMViewer.CrossCutting.Security; // Assuming a secure storage utility from REPO-CROSS-CUTTING

namespace TheSSS.DICOMViewer.Integration.Services
{
    public class CredentialManager : ICredentialManager
    {
        private readonly CredentialManagerSettings _settings;
        private readonly ILoggerAdapter<CredentialManager> _logger;
        // private readonly ISecureStorageService _secureStorageService; // Example dependency for secure storage

        public CredentialManager(
            IOptions<CredentialManagerSettings> settings,
            ILoggerAdapter<CredentialManager> logger
            // ISecureStorageService secureStorageService // Inject if using a specific secure store
            )
        {
            _settings = settings.Value;
            _logger = logger;
            // _secureStorageService = secureStorageService;
        }

        public Task<ServiceCredentials> GetCredentialsAsync(string serviceIdentifier, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to retrieve credentials for service: {ServiceIdentifier}", serviceIdentifier);

            if (string.IsNullOrWhiteSpace(serviceIdentifier))
            {
                _logger.LogError("Service identifier cannot be null or empty.");
                throw new ArgumentNullException(nameof(serviceIdentifier));
            }

            if (_settings.ServiceCredentialMappings == null || !_settings.ServiceCredentialMappings.TryGetValue(serviceIdentifier, out var credentialConfig))
            {
                _logger.LogError("No credential mapping found for service identifier: {ServiceIdentifier}", serviceIdentifier);
                throw new InvalidOperationException($"Credential configuration not found for service: {serviceIdentifier}");
            }

            ServiceCredentials credentials = null;

            switch (credentialConfig.SourceType.ToLowerInvariant())
            {
                case "environment":
                    credentials = GetFromEnvironment(credentialConfig, serviceIdentifier);
                    break;
                case "configuration": // Plain text in config - less secure, use for non-sensitive or dev
                    credentials = GetFromConfiguration(credentialConfig, serviceIdentifier);
                    break;
                case "dpapi": // Example using a secure store wrapper
                    // credentials = await _secureStorageService.GetCredentialsAsync(credentialConfig.StorageKey, cancellationToken);
                    _logger.LogWarning("DPAPI or dedicated secure storage not fully implemented in this example CredentialManager. Using placeholder logic for {ServiceIdentifier}.", serviceIdentifier);
                    // Placeholder for DPAPI or other secure store:
                    credentials = new ServiceCredentials // Dummy credentials for example
                    {
                        Username = $"{serviceIdentifier}_user_dpapi_placeholder",
                        Password = "dpapi_secure_password_placeholder",
                        ApiKey = "dpapi_api_key_placeholder"
                    };
                    break;
                default:
                    _logger.LogError("Unsupported credential source type '{SourceType}' for service: {ServiceIdentifier}", credentialConfig.SourceType, serviceIdentifier);
                    throw new NotSupportedException($"Credential source type '{credentialConfig.SourceType}' is not supported.");
            }
            
            if (credentials == null)
            {
                 _logger.LogError("Failed to retrieve credentials for service: {ServiceIdentifier} from source: {SourceType}", serviceIdentifier, credentialConfig.SourceType);
                 throw new InvalidOperationException($"Could not retrieve credentials for {serviceIdentifier}.");
            }

            _logger.LogInformation("Successfully retrieved credentials for service: {ServiceIdentifier} from {SourceType}", serviceIdentifier, credentialConfig.SourceType);
            return Task.FromResult(credentials);
        }

        private ServiceCredentials GetFromEnvironment(CredentialSourceConfig config, string serviceIdentifier)
        {
            var username = Environment.GetEnvironmentVariable(config.UsernameKey ?? $"{serviceIdentifier}_USERNAME");
            var password = Environment.GetEnvironmentVariable(config.PasswordKey ?? $"{serviceIdentifier}_PASSWORD");
            var apiKey = Environment.GetEnvironmentVariable(config.ApiKeyKey ?? $"{serviceIdentifier}_APIKEY");
            var token = Environment.GetEnvironmentVariable(config.TokenKey ?? $"{serviceIdentifier}_TOKEN");

            if (string.IsNullOrEmpty(username) && string.IsNullOrEmpty(apiKey) && string.IsNullOrEmpty(token))
            {
                _logger.LogWarning("No environment credentials found for {ServiceIdentifier} using keys: User={UserKey}, Pass={PassKey}, ApiKey={ApiKeyKey}, Token={TokenKey}",
                    serviceIdentifier, config.UsernameKey, config.PasswordKey, config.ApiKeyKey, config.TokenKey);
                return null;
            }

            return new ServiceCredentials
            {
                Username = username,
                Password = password,
                ApiKey = apiKey,
                Token = token
            };
        }

        private ServiceCredentials GetFromConfiguration(CredentialSourceConfig config, string serviceIdentifier)
        {
             // Directly from config.ServiceCredentialMappings[serviceIdentifier].Values - USE WITH CAUTION for production
            if (config.Values == null || 
                (!config.Values.ContainsKey("Username") && !config.Values.ContainsKey("ApiKey") && !config.Values.ContainsKey("Token")) )
            {
                 _logger.LogWarning("No direct configuration credentials found for {ServiceIdentifier} in CredentialManagerSettings.Values", serviceIdentifier);
                 return null;
            }

            return new ServiceCredentials
            {
                Username = config.Values.TryGetValue("Username", out var user) ? user : null,
                Password = config.Values.TryGetValue("Password", out var pass) ? pass : null,
                ApiKey = config.Values.TryGetValue("ApiKey", out var apiKey) ? apiKey : null,
                Token = config.Values.TryGetValue("Token", out var tokenVal) ? tokenVal : null
            };
        }
    }
}