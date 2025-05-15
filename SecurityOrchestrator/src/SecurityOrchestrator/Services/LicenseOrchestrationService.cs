using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TheSSS.DICOMViewer.Security.Configuration;
using TheSSS.DICOMViewer.Security.DTOs;
using TheSSS.DICOMViewer.Security.Exceptions;
using TheSSS.DICOMViewer.Security.Interfaces;
using TheSSS.DICOMViewer.Security.Validators;

namespace TheSSS.DICOMViewer.Security.Services
{
    public class LicenseOrchestrationService : ILicenseOrchestrationService
    {
        private readonly ILicenseApiClient _licenseApiClient;
        private readonly IAuditLogService _auditLogService;
        private readonly IAlertingService _alertingService;
        private readonly SecurityOrchestratorSettings _settings;
        private readonly LicenseKeyValidator _licenseKeyValidator;

        // Placeholder for storing/retrieving the current license key and machine ID
        // In a real application, this would come from a secure configuration store or be managed more robustly.
        private string _currentConfiguredLicenseKey = "YOUR_CONFIGURED_LICENSE_KEY"; // REQ-LDM-LIC-002 (partially, needs secure storage)
        private string _machineId;

        public LicenseOrchestrationService(
            ILicenseApiClient licenseApiClient,
            IAuditLogService auditLogService,
            IAlertingService alertingService,
            IOptions<SecurityOrchestratorSettings> settingsOptions)
        {
            _licenseApiClient = licenseApiClient ?? throw new ArgumentNullException(nameof(licenseApiClient));
            _auditLogService = auditLogService ?? throw new ArgumentNullException(nameof(auditLogService));
            _alertingService = alertingService ?? throw new ArgumentNullException(nameof(alertingService));
            _settings = settingsOptions?.Value ?? throw new ArgumentNullException(nameof(settingsOptions));
            _licenseKeyValidator = new LicenseKeyValidator();
            _machineId = GenerateMachineIdentifier(_settings.MachineIdentifierSource);
        }

        private string GenerateMachineIdentifier(string? source) // REQ-LDM-LIC-002
        {
            // Placeholder: In a real scenario, this would use a robust method to get a unique machine ID
            // based on the MachineIdentifierSource configuration.
            // For example, using MAC address, CPU ID, or a GUID stored in the registry.
            if (!string.IsNullOrEmpty(source) && source.Equals("MAC_ADDRESS_HASH", StringComparison.OrdinalIgnoreCase))
            {
                // Example: Get first MAC address and hash it. Needs more robust error handling and selection.
                try
                {
                    // This is a simplified example and might not work on all platforms or have permission issues.
                    var macAddr = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()?
                        .FirstOrDefault(nic => nic.OperationalStatus == System.Net.NetworkInformation.OperationalStatus.Up && nic.NetworkInterfaceType != System.Net.NetworkInformation.NetworkInterfaceType.Loopback)?
                        .GetPhysicalAddress()?.ToString();
                    if (!string.IsNullOrEmpty(macAddr))
                    {
                        using var sha256 = SHA256.Create();
                        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(macAddr));
                        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                    }
                }
                catch { /* Fallback to default */ }
            }
            // Default or fallback
            return Environment.MachineName + "_" + Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }

        public async Task<LicenseValidationResultDto> ValidateLicenseOnStartupAsync()
        {
            var licenseKeyToValidate = _currentConfiguredLicenseKey; // This should be retrieved securely
            var eventDetails = new SecurityEventDetailsDto(
                EventType: "LicenseValidationStartup",
                UserId: "SYSTEM",
                Timestamp: DateTime.UtcNow,
                Outcome: "Attempt",
                Details: $"Attempting license validation on startup. Key: {GetPartialKey(licenseKeyToValidate)}, MachineID: {_machineId.Substring(0, Math.Min(_machineId.Length, 8))}...",
                SourceIP: null
            );
            await _auditLogService.LogEventAsync(eventDetails); // REQ-7-001

            try
            {
                var requestDto = new LicenseValidationRequestDto(licenseKeyToValidate, _machineId);
                var validationResult = await _licenseApiClient.ValidateLicenseAsync(requestDto); // REQ-LDM-LIC-002

                if (!validationResult.IsValid || (validationResult.ExpiryDate.HasValue && validationResult.ExpiryDate.Value < DateTime.UtcNow.AddDays(_settings.LicenseExpiryWarningDays)))
                {
                    var alertMessage = !validationResult.IsValid
                        ? $"License validation failed on startup: {validationResult.StatusMessage}"
                        : $"License is expiring soon: {validationResult.ExpiryDate.Value:yyyy-MM-dd}. Status: {validationResult.StatusMessage}";
                    
                    await _alertingService.RaiseAlertAsync(new AlertDetailsDto( // REQ-LDM-LIC-005
                        Severity: "Critical",
                        Message: alertMessage,
                        SourceComponent: "LicenseOrchestrationService",
                        ErrorCode: "LICENSE_VALIDATION_ERROR"
                    ));
                }
                
                eventDetails = eventDetails with { Outcome = validationResult.IsValid ? "Success" : "Failure", Details = $"License validation on startup result: {validationResult.StatusMessage}, Valid: {validationResult.IsValid}, Expiry: {validationResult.ExpiryDate?.ToString("o")}" };
                await _auditLogService.LogEventAsync(eventDetails); // REQ-7-001
                return validationResult;
            }
            catch (Exception ex)
            {
                var errorMsg = $"Critical error during startup license validation: {ex.Message}";
                eventDetails = eventDetails with { Outcome = "Error", Details = errorMsg };
                await _auditLogService.LogEventAsync(eventDetails); // REQ-7-001

                await _alertingService.RaiseAlertAsync(new AlertDetailsDto( // REQ-LDM-LIC-005 (for failure)
                    Severity: "Critical",
                    Message: errorMsg,
                    SourceComponent: "LicenseOrchestrationService",
                    ErrorCode: "LICENSE_VALIDATION_EXCEPTION"
                ));
                throw new LicenseValidationFailedException(errorMsg, ex);
            }
        }

        public async Task<LicenseValidationResultDto> PerformPeriodicLicenseCheckAsync()
        {
            if (!_settings.EnablePeriodicLicenseCheck)
            {
                return new LicenseValidationResultDto(true, "Periodic check disabled.", null, null);
            }

            var licenseKeyToValidate = _currentConfiguredLicenseKey; // This should be retrieved securely
             var eventDetails = new SecurityEventDetailsDto(
                EventType: "LicenseValidationPeriodic",
                UserId: "SYSTEM",
                Timestamp: DateTime.UtcNow,
                Outcome: "Attempt",
                Details: $"Attempting periodic license check. Key: {GetPartialKey(licenseKeyToValidate)}, MachineID: {_machineId.Substring(0, Math.Min(_machineId.Length, 8))}...",
                SourceIP: null
            );
            await _auditLogService.LogEventAsync(eventDetails); // REQ-7-001


            try
            {
                var requestDto = new LicenseValidationRequestDto(licenseKeyToValidate, _machineId);
                var validationResult = await _licenseApiClient.ValidateLicenseAsync(requestDto); // REQ-LDM-LIC-002

                if (!validationResult.IsValid || (validationResult.ExpiryDate.HasValue && validationResult.ExpiryDate.Value < DateTime.UtcNow.AddDays(_settings.LicenseExpiryWarningDays)))
                {
                     var alertMessage = !validationResult.IsValid
                        ? $"Periodic license check failed: {validationResult.StatusMessage}"
                        : $"License is expiring soon (periodic check): {validationResult.ExpiryDate.Value:yyyy-MM-dd}. Status: {validationResult.StatusMessage}";

                    await _alertingService.RaiseAlertAsync(new AlertDetailsDto( // REQ-LDM-LIC-005
                        Severity: "Warning", // Or Critical depending on policy
                        Message: alertMessage,
                        SourceComponent: "LicenseOrchestrationService",
                        ErrorCode: "LICENSE_PERIODIC_CHECK_ERROR"
                    ));
                }
                
                eventDetails = eventDetails with { Outcome = validationResult.IsValid ? "Success" : "Failure", Details = $"Periodic license check result: {validationResult.StatusMessage}, Valid: {validationResult.IsValid}, Expiry: {validationResult.ExpiryDate?.ToString("o")}" };
                await _auditLogService.LogEventAsync(eventDetails); // REQ-7-001
                return validationResult;
            }
            catch (Exception ex)
            {
                var errorMsg = $"Error during periodic license check: {ex.Message}";
                eventDetails = eventDetails with { Outcome = "Error", Details = errorMsg };
                await _auditLogService.LogEventAsync(eventDetails); // REQ-7-001

                await _alertingService.RaiseAlertAsync(new AlertDetailsDto( // REQ-LDM-LIC-005 (for failure)
                    Severity: "Error",
                    Message: errorMsg,
                    SourceComponent: "LicenseOrchestrationService",
                    ErrorCode: "LICENSE_PERIODIC_EXCEPTION"
                ));
                // Decide if this should throw or just return a failed DTO
                // For periodic check, might be better to not crash the app if it was running
                return new LicenseValidationResultDto(false, $"Exception: {ex.Message}", null, null);
            }
        }

        public async Task<LicenseActivationResultDto> ActivateLicenseAsync(string licenseKey)
        {
            var validationResult = _licenseKeyValidator.Validate(licenseKey);
            if (!validationResult.IsValid)
            {
                var errorMessage = $"Invalid license key format: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}";
                await _auditLogService.LogEventAsync(new SecurityEventDetailsDto( // REQ-7-001
                    EventType: "LicenseActivation",
                    UserId: "SYSTEM", // Or user initiating if interactive
                    Timestamp: DateTime.UtcNow,
                    Outcome: "Failure",
                    Details: $"License activation attempt failed due to invalid key format. Key: {GetPartialKey(licenseKey)}",
                    SourceIP: null
                ));
                throw new LicenseValidationFailedException(errorMessage);
            }

            var eventDetails = new SecurityEventDetailsDto(
                EventType: "LicenseActivation",
                UserId: "SYSTEM", // Or actual user if applicable
                Timestamp: DateTime.UtcNow,
                Outcome: "Attempt",
                Details: $"Attempting license activation. Key: {GetPartialKey(licenseKey)}, MachineID: {_machineId.Substring(0, Math.Min(_machineId.Length, 8))}...",
                SourceIP: null
            );
            await _auditLogService.LogEventAsync(eventDetails); // REQ-7-001

            try
            {
                // REQ-LDM-LIC-004 (Odoo API interaction is via ILicenseApiClient)
                var activationResult = await _licenseApiClient.ActivateLicenseAsync(licenseKey, _machineId);

                if (activationResult.IsSuccess && activationResult.ValidatedLicenseInfo != null)
                {
                    // Successfully activated, update stored license information
                    _currentConfiguredLicenseKey = licenseKey; // Placeholder for actual secure storage update
                    // Potentially trigger an event or mechanism to update configuration elsewhere
                    // For example, `IConfigurationWriter.Set("License:CurrentKey", licenseKey)`
                    Console.WriteLine($"License activated. New key {_currentConfiguredLicenseKey} stored (placeholder).");
                }
                
                eventDetails = eventDetails with { Outcome = activationResult.IsSuccess ? "Success" : "Failure", Details = $"License activation result: {activationResult.Message}" };
                await _auditLogService.LogEventAsync(eventDetails); // REQ-7-001
                return activationResult;
            }
            catch (Exception ex)
            {
                var errorMsg = $"Critical error during license activation: {ex.Message}";
                eventDetails = eventDetails with { Outcome = "Error", Details = errorMsg };
                await _auditLogService.LogEventAsync(eventDetails); // REQ-7-001
                throw new LicenseValidationFailedException(errorMsg, ex);
            }
        }

        private string GetPartialKey(string key)
        {
            if (string.IsNullOrEmpty(key)) return "EMPTY_KEY";
            if (key.Length <= 8) return key;
            return $"{key.Substring(0, 4)}...{key.Substring(key.Length - 4)}";
        }
    }
}