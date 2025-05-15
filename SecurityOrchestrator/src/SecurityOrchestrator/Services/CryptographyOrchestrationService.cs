using System;
using System.Security.Cryptography; // For DataProtectionScope if ISensitiveDataProtector uses it directly
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TheSSS.DICOMViewer.Security.Configuration;
using TheSSS.DICOMViewer.Security.DTOs;
using TheSSS.DICOMViewer.Security.Exceptions;
using TheSSS.DICOMViewer.Security.Interfaces;

namespace TheSSS.DICOMViewer.Security.Services
{
    public class CryptographyOrchestrationService : ICryptographyOrchestrationService
    {
        private readonly ISensitiveDataProtector _sensitiveDataProtector;
        private readonly IAuditLogService _auditLogService;
        private readonly SecurityOrchestratorSettings _settings;

        public CryptographyOrchestrationService(
            ISensitiveDataProtector sensitiveDataProtector,
            IAuditLogService auditLogService,
            IOptions<SecurityOrchestratorSettings> settingsOptions)
        {
            _sensitiveDataProtector = sensitiveDataProtector ?? throw new ArgumentNullException(nameof(sensitiveDataProtector));
            _auditLogService = auditLogService ?? throw new ArgumentNullException(nameof(auditLogService));
            _settings = settingsOptions?.Value ?? throw new ArgumentNullException(nameof(settingsOptions));
        }

        public async Task<SensitiveDataProtectionResultDto> ProtectDataAsync(SensitiveDataProtectionRequestDto request)
        {
            if (request == null || request.DataToProtect == null)
            {
                throw new ArgumentNullException(nameof(request), "Request and data to protect cannot be null.");
            }

            var protectionScope = request.ProtectionScope ?? _settings.DefaultDataProtectionScope; // REQ-7-017 (Default Scope)
            
            var eventDetails = new SecurityEventDetailsDto( // REQ-7-001
                EventType: "DataProtection",
                UserId: request.UserId ?? "SYSTEM", // Assuming DTO has UserId, or it's a system operation
                Timestamp: DateTime.UtcNow,
                Outcome: "Attempt",
                Details: $"Attempting to protect data. Scope: {protectionScope}, Size: {request.DataToProtect.Length} bytes.",
                SourceIP: request.ClientIpAddress // Assuming DTO has this
            );
            await _auditLogService.LogEventAsync(eventDetails);


            try
            {
                // REQ-7-017: Orchestrates encryption of sensitive data
                byte[] protectedData = await _sensitiveDataProtector.ProtectAsync(request.DataToProtect, request.Entropy, protectionScope);
                var resultDto = new SensitiveDataProtectionResultDto(protectedData);

                eventDetails = eventDetails with { Outcome = "Success", Details = $"Data protection successful. Scope: {protectionScope}, Original Size: {request.DataToProtect.Length}, Protected Size: {protectedData.Length}."};
                await _auditLogService.LogEventAsync(eventDetails); // REQ-7-001
                return resultDto;
            }
            catch (Exception ex)
            {
                var errorMsg = $"Error during data protection: {ex.Message}";
                eventDetails = eventDetails with { Outcome = "Error", Details = errorMsg };
                await _auditLogService.LogEventAsync(eventDetails); // REQ-7-001
                throw new CryptographyOperationException(errorMsg, ex);
            }
        }

        public async Task<SensitiveDataUnprotectionResultDto> UnprotectDataAsync(SensitiveDataUnprotectionRequestDto request)
        {
            if (request == null || request.ProtectedData == null)
            {
                throw new ArgumentNullException(nameof(request), "Request and protected data cannot be null.");
            }

            var protectionScope = request.ProtectionScope ?? _settings.DefaultDataProtectionScope; // REQ-7-017 (Default Scope)

            var eventDetails = new SecurityEventDetailsDto( // REQ-7-001
                EventType: "DataUnprotection",
                UserId: request.UserId ?? "SYSTEM", // Assuming DTO has UserId
                Timestamp: DateTime.UtcNow,
                Outcome: "Attempt",
                Details: $"Attempting to unprotect data. Scope: {protectionScope}, Size: {request.ProtectedData.Length} bytes.",
                SourceIP: request.ClientIpAddress // Assuming DTO has this
            );
            await _auditLogService.LogEventAsync(eventDetails);

            try
            {
                // REQ-7-017: Orchestrates decryption of sensitive data
                byte[] unprotectedData = await _sensitiveDataProtector.UnprotectAsync(request.ProtectedData, request.Entropy, protectionScope);
                var resultDto = new SensitiveDataUnprotectionResultDto(unprotectedData);

                eventDetails = eventDetails with { Outcome = "Success", Details = $"Data unprotection successful. Scope: {protectionScope}, Protected Size: {request.ProtectedData.Length}, Unprotected Size: {unprotectedData.Length}."};
                await _auditLogService.LogEventAsync(eventDetails); // REQ-7-001
                return resultDto;
            }
            catch (CryptographicException ex) // Catch more specific crypto errors if ISensitiveDataProtector throws them
            {
                var errorMsg = $"Cryptographic error during data unprotection (e.g., incorrect entropy/scope, corrupted data): {ex.Message}";
                eventDetails = eventDetails with { Outcome = "Failure", Details = errorMsg };
                await _auditLogService.LogEventAsync(eventDetails); // REQ-7-001
                throw new CryptographyOperationException(errorMsg, ex);
            }
            catch (Exception ex)
            {
                var errorMsg = $"Error during data unprotection: {ex.Message}";
                eventDetails = eventDetails with { Outcome = "Error", Details = errorMsg };
                await _auditLogService.LogEventAsync(eventDetails); // REQ-7-001
                throw new CryptographyOperationException(errorMsg, ex);
            }
        }
    }
}