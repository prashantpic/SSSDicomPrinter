using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TheSSS.DICOMViewer.Integration.Configuration;
using TheSSS.DICOMViewer.Integration.Interfaces;
using TheSSS.DICOMViewer.Integration.Models;
using TheSSS.DICOMViewer.Common.Interfaces; // Assuming ILoggerAdapter and ISecureDataStorage are here
using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory; // For a more robust cache

namespace TheSSS.DICOMViewer.Integration.Services;

public class CredentialManager : ICredentialManager, IDisposable
{
    private readonly CredentialManagerSettings _settings;
    private readonly ILoggerAdapter _logger;
    private readonly ISecureDataStorage? _secureStorage; // Nullable if some store types don't need it
    private readonly IMemoryCache _cache; // Using IMemoryCache for better caching control

    public CredentialManager(
        IOptions<CredentialManagerSettings> settings,
        ILoggerAdapter logger,
        IMemoryCache memoryCache,
        IServiceProvider serviceProvider) // To optionally resolve ISecureDataStorage
    {
        _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _cache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));

        // Conditionally resolve ISecureDataStorage if DPAPI or similar store is configured
        if (_settings.StoreType.Equals("DPAPI", StringComparison.OrdinalIgnoreCase) ||
            _settings.StoreType.Equals("AzureKeyVault", StringComparison.OrdinalIgnoreCase)) // Example
        {
            _secureStorage = (ISecureDataStorage?)serviceProvider.GetService(typeof(ISecureDataStorage));
            if (_secureStorage == null)
            {
                _logger.Error($"Secure storage type '{_settings.StoreType}' is configured, but ISecureDataStorage service is not registered.");
                // This could be a fatal error depending on requirements.
                // For now, allow it to proceed, but GetCredentialsAsync will fail for these types.
            }
        }
        
        if (string.IsNullOrWhiteSpace(_settings.StoreType))
        {
            _logger.Warning("CredentialManager StoreType is not configured. Defaulting to EnvironmentVariables.");
            _settings.StoreType = "EnvironmentVariables";
        }
    }

    public async Task<ServiceCredentials> GetCredentialsAsync(string serviceIdentifier, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(serviceIdentifier))
        {
            throw new ArgumentException("Service identifier cannot be null or empty.", nameof(serviceIdentifier));
        }

        string cacheKey = $"ServiceCredentials_{serviceIdentifier}_{_settings.StoreType}";

        // Try to get from cache
        if (_cache.TryGetValue(cacheKey, out ServiceCredentials? cachedCredentials) && cachedCredentials != null)
        {
             _logger.Debug($"Returning cached credentials for service '{serviceIdentifier}'.");
             return cachedCredentials;
        }

        _logger.Information($"Retrieving credentials for service '{serviceIdentifier}' using store type: {_settings.StoreType}.");

        ServiceCredentials credentials;
        try
        {
            switch (_settings.StoreType.ToLowerInvariant())
            {
                case "environmentvariables":
                    credentials = GetCredentialsFromEnvironmentVariables(serviceIdentifier);
                    break;
                case "dpapi":
                    if (_secureStorage == null) throw new InvalidOperationException("DPAPI store type configured, but ISecureDataStorage is not available.");
                    credentials = await GetCredentialsFromSecureStorageAsync(_secureStorage, serviceIdentifier, "DPAPI", cancellationToken);
                    break;
                // Example for Azure Key Vault (conceptual)
                // case "azurekeyvault":
                //     if (_secureStorage == null) throw new InvalidOperationException("AzureKeyVault store type configured, but ISecureDataStorage is not available (needs specific KV client).");
                //     credentials = await GetCredentialsFromSecureStorageAsync(_secureStorage, serviceIdentifier, "AzureKeyVault", cancellationToken);
                //     break;
                default:
                    throw new NotSupportedException($"Credential store type '{_settings.StoreType}' is not supported.");
            }

            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(_settings.CredentialCacheDuration > TimeSpan.Zero ? _settings.CredentialCacheDuration : TimeSpan.FromMinutes(5)); // Default cache if not set

            _cache.Set(cacheKey, credentials, cacheEntryOptions);
            _logger.Information($"Successfully retrieved and cached credentials for service '{serviceIdentifier}'. Cache expires in {_settings.CredentialCacheDuration}.");

            // TODO: Implement credential rotation checks if _settings.EnableCredentialRotationChecks is true.
            // This might involve:
            // 1. Storing credential acquisition time.
            // 2. Periodically (e.g., on next GetCredentialsAsync after some interval, or background task) re-fetching.
            // 3. If _settings.EnableCredentialRotationChecks is true and cache duration is relatively short,
            //    the natural cache expiry and re-fetch might suffice for some rotation scenarios.
            //    A more proactive check would require comparing with an expected rotation schedule or version.

            return credentials;
        }
        catch (CredentialRetrievalException) { throw; } // Re-throw custom exception
        catch (Exception ex)
        {
            _logger.Error(ex, $"Failed to retrieve credentials for service '{serviceIdentifier}' from store type '{_settings.StoreType}'.");
            throw new CredentialRetrievalException($"Failed to retrieve credentials for service '{serviceIdentifier}'. See inner exception.", ex);
        }
    }

    private ServiceCredentials GetCredentialsFromEnvironmentVariables(string serviceIdentifier)
    {
        string? username = Environment.GetEnvironmentVariable($"{serviceIdentifier.ToUpperInvariant()}_USERNAME");
        string? password = Environment.GetEnvironmentVariable($"{serviceIdentifier.ToUpperInvariant()}_PASSWORD");
        string? apiKey = Environment.GetEnvironmentVariable($"{serviceIdentifier.ToUpperInvariant()}_APIKEY");
        string? token = Environment.GetEnvironmentVariable($"{serviceIdentifier.ToUpperInvariant()}_TOKEN");

        if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(apiKey) && string.IsNullOrWhiteSpace(token) && string.IsNullOrWhiteSpace(password))
        {
            _logger.Warning($"No environment variables found for service '{serviceIdentifier}'. Conventions: {serviceIdentifier.ToUpperInvariant()}_USERNAME, _PASSWORD, _APIKEY, _TOKEN.");
            // Consider if this should be an error or return empty credentials
            throw new CredentialRetrievalException($"No environment variable credentials found for '{serviceIdentifier}'.");
        }
        return new ServiceCredentials(username, password, apiKey, token);
    }

    private async Task<ServiceCredentials> GetCredentialsFromSecureStorageAsync(
        ISecureDataStorage storage, string serviceIdentifier, string storeName, CancellationToken cancellationToken)
    {
        // Convention for secret keys in secure store: e.g., "ServiceIntegrationGateway_OdooApi_Username"
        string prefix = $"ServiceIntegrationGateway_{serviceIdentifier}_";
        
        string? username = await storage.RetrieveSecretAsync($"{prefix}Username", cancellationToken).ConfigureAwait(false);
        string? password = await storage.RetrieveSecretAsync($"{prefix}Password", cancellationToken).ConfigureAwait(false);
        string? apiKey = await storage.RetrieveSecretAsync($"{prefix}ApiKey", cancellationToken).ConfigureAwait(false);
        string? token = await storage.RetrieveSecretAsync($"{prefix}Token", cancellationToken).ConfigureAwait(false);

        if (string.IsNullOrWhiteSpace(username) && string.IsNullOrWhiteSpace(apiKey) && string.IsNullOrWhiteSpace(token) && string.IsNullOrWhiteSpace(password))
        {
            _logger.Warning($"No credentials found in secure store '{storeName}' for identifier '{serviceIdentifier}' with prefix '{prefix}'.");
            throw new CredentialRetrievalException($"No credentials found in secure store '{storeName}' for '{serviceIdentifier}'.");
        }
        return new ServiceCredentials(username, password, apiKey, token);
    }
    
    public void Dispose()
    {
        // If _cache implements IDisposable (like MemoryCache does), it's managed by DI container.
        // No explicit disposal needed here unless this class creates its own IDisposable resources.
        GC.SuppressFinalize(this);
    }
}

// Assume this interface and a concrete implementation (e.g., DpapiSecureDataStorage)
// are provided by REPO-CROSS-CUTTING and registered in DI.
// namespace TheSSS.DICOMViewer.Common.Interfaces
// {
//     public interface ISecureDataStorage
//     {
//         Task<string?> RetrieveSecretAsync(string key, CancellationToken cancellationToken = default);
//         Task StoreSecretAsync(string key, string secret, CancellationToken cancellationToken = default);
//     }
// }