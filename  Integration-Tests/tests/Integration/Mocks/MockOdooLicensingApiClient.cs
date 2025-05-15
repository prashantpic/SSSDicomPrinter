using System.Collections.Concurrent;
using TheSSS.DicomViewer.Application; // Assuming ILicensingApiClient, LicenseActivationResult, LicenseValidationResult are here
// If result types are in Domain: using TheSSS.DicomViewer.Domain;

namespace TheSSS.DicomViewer.IntegrationTests.Mocks
{
    // These would typically be defined in TheSSS.DicomViewer.Application or TheSSS.DicomViewer.Domain
    // For compilation purposes if not available in this context, define minimal structures:
    /*
    namespace TheSSS.DicomViewer.Application
    {
        public class LicenseActivationResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            // Add other relevant properties like ActivatedFeatures, ExpiryDate, etc.
        }

        public class LicenseValidationResult
        {
            public bool IsValid { get; set; }
            public string Message { get; set; }
            public DateTime? ExpiryDate { get; set; }
            public bool IsExpired { get; set; }
            public bool IsInGracePeriod { get; set; }
            // Add other relevant properties
        }

        public interface ILicensingApiClient
        {
            Task<LicenseActivationResult> ActivateLicenseAsync(string licenseKey, CancellationToken cancellationToken = default);
            Task<LicenseValidationResult> ValidateLicenseAsync(string licenseKey, CancellationToken cancellationToken = default);
        }
    }
    */


    public class MockOdooLicensingApiClient : ILicensingApiClient
    {
        private readonly ConcurrentDictionary<string, LicenseActivationResult> _activationResponses = new();
        private readonly ConcurrentDictionary<string, LicenseValidationResult> _validationResponses = new();

        private LicenseActivationResult _defaultActivationResult = new() { Success = false, Message = "Default mock activation failure." };
        private LicenseValidationResult _defaultValidationResult = new() { IsValid = false, Message = "Default mock validation failure." };

        public Task<LicenseActivationResult> ActivateLicenseAsync(string licenseKey, CancellationToken cancellationToken = default)
        {
            if (_activationResponses.TryGetValue(licenseKey, out var specificResult))
            {
                return Task.FromResult(specificResult);
            }
            return Task.FromResult(_defaultActivationResult);
        }

        public Task<LicenseValidationResult> ValidateLicenseAsync(string licenseKey, CancellationToken cancellationToken = default)
        {
            if (_validationResponses.TryGetValue(licenseKey, out var specificResult))
            {
                return Task.FromResult(specificResult);
            }
            return Task.FromResult(_defaultValidationResult);
        }

        public void SetupActivationResponse(string licenseKey, LicenseActivationResult result)
        {
            _activationResponses[licenseKey] = result;
        }

        public void SetupValidationResponse(string licenseKey, LicenseValidationResult result)
        {
            _validationResponses[licenseKey] = result;
        }

        public void SetDefaultActivationResponse(LicenseActivationResult result)
        {
            _defaultActivationResult = result;
        }

        public void SetDefaultValidationResponse(LicenseValidationResult result)
        {
            _defaultValidationResult = result;
        }

        public void Reset()
        {
            _activationResponses.Clear();
            _validationResponses.Clear();
            _defaultActivationResult = new() { Success = false, Message = "Default mock activation failure." };
            _defaultValidationResult = new() { IsValid = false, Message = "Default mock validation failure." };
        }
    }
}