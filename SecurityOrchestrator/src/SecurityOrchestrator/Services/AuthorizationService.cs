using System;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Security.DTOs;
using TheSSS.DICOMViewer.Security.Engines;
using TheSSS.DICOMViewer.Security.Exceptions;
using TheSSS.DICOMViewer.Security.Interfaces;

namespace TheSSS.DICOMViewer.Security.Services
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly SecurityPolicyEngine _securityPolicyEngine;
        private readonly IAuditLogService _auditLogService;

        public AuthorizationService(
            SecurityPolicyEngine securityPolicyEngine,
            IAuditLogService auditLogService)
        {
            _securityPolicyEngine = securityPolicyEngine ?? throw new ArgumentNullException(nameof(securityPolicyEngine));
            _auditLogService = auditLogService ?? throw new ArgumentNullException(nameof(auditLogService));
        }

        public async Task<AuthorizationResultDto> IsAuthorizedAsync(AuthorizationRequestDto request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            if (string.IsNullOrWhiteSpace(request.UserId) || string.IsNullOrWhiteSpace(request.Permission))
            {
                // Basic validation, more specific validation can be added if needed.
                // FluentValidation could be used here too if desired, by validating the DTO earlier or injecting a validator.
                 await _auditLogService.LogEventAsync(new SecurityEventDetailsDto( // REQ-7-001
                    EventType: "AuthorizationCheck",
                    UserId: request.UserId ?? "UNKNOWN_USER",
                    Timestamp: DateTime.UtcNow,
                    Outcome: "ValidationFailure",
                    Details: "Authorization request is invalid (missing UserId or Permission).",
                    SourceIP: request.ClientIpAddress // Assuming DTO has this
                ));
                throw new ArgumentException("UserId and Permission must be provided in AuthorizationRequestDto.");
            }

            var eventDetails = new SecurityEventDetailsDto( // REQ-7-001
                EventType: "AuthorizationCheck",
                UserId: request.UserId,
                Timestamp: DateTime.UtcNow,
                Outcome: "Attempt",
                Details: $"User '{request.UserId}' checking permission '{request.Permission}' on resource '{request.ResourceContext ?? "N/A"}'.",
                SourceIP: request.ClientIpAddress // Assuming AuthorizationRequestDto contains ClientIpAddress
            );
            await _auditLogService.LogEventAsync(eventDetails);

            try
            {
                // REQ-7-005: Determines if a user is authorized to perform a specific action based on RBAC
                bool isAllowed = await _securityPolicyEngine.EvaluatePermissionAsync(request.UserId, request.Permission, request.ResourceContext);

                var resultDto = new AuthorizationResultDto(isAllowed, isAllowed ? "Authorized" : "Denied");
                
                eventDetails = eventDetails with { Outcome = isAllowed ? "Authorized" : "Denied", Details = $"Authorization check for User '{request.UserId}', Permission '{request.Permission}', Resource '{request.ResourceContext ?? "N/A"}' resulted in: {resultDto.Reason}" };
                await _auditLogService.LogEventAsync(eventDetails); // REQ-7-001

                if (!isAllowed)
                {
                    // As per SDS: "Returns AuthorizationResultDto indicating success or failure and reason."
                    // "Throws AuthorizationFailedException or SecurityOrchestrationException on internal errors during the check."
                    // This means if evaluation logic itself fails, an exception. If policy denies, return DTO.
                    // The current flow is to return the DTO indicating denial.
                }
                return resultDto;

            }
            catch (Exception ex)
            {
                var errorMsg = $"Error during authorization check for user '{request.UserId}', permission '{request.Permission}': {ex.Message}";
                eventDetails = eventDetails with { Outcome = "Error", Details = errorMsg };
                await _auditLogService.LogEventAsync(eventDetails); // REQ-7-001
                // Throws AuthorizationFailedException or SecurityOrchestrationException on internal errors.
                throw new AuthorizationFailedException(errorMsg, ex);
            }
        }
    }
}