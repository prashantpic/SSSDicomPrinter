using System;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Security.DTOs;
using TheSSS.DICOMViewer.Security.Exceptions;
using TheSSS.DICOMViewer.Security.Interfaces;
using TheSSS.DICOMViewer.Security.Validators; // Assuming AuthenticationRequestValidator is for DTO

namespace TheSSS.DICOMViewer.Security.Services
{
    public class AuthenticationOrchestrationService : IAuthenticationOrchestrationService
    {
        private readonly IIdentityProviderService _identityProviderService;
        private readonly IAuditLogService _auditLogService;
        private readonly AuthenticationRequestValidator _authenticationRequestValidator;


        public AuthenticationOrchestrationService(
            IIdentityProviderService identityProviderService,
            IAuditLogService auditLogService)
        {
            _identityProviderService = identityProviderService ?? throw new ArgumentNullException(nameof(identityProviderService));
            _auditLogService = auditLogService ?? throw new ArgumentNullException(nameof(auditLogService));
            _authenticationRequestValidator = new AuthenticationRequestValidator();
        }

        public async Task<AuthenticationResultDto> AuthenticateAsync(AuthenticationRequestDto request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var validationResult = _authenticationRequestValidator.Validate(request);
            if (!validationResult.IsValid)
            {
                var errorMessage = $"Authentication request validation failed: {string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))}";
                 await _auditLogService.LogEventAsync(new SecurityEventDetailsDto( // REQ-7-001
                    EventType: "AuthenticationAttempt",
                    UserId: request.Username, // Use username from request for failed validation log
                    Timestamp: DateTime.UtcNow,
                    Outcome: "ValidationFailure",
                    Details: errorMessage,
                    SourceIP: request.ClientIpAddress // Assuming DTO has this
                ));
                throw new AuthenticationFailedException(errorMessage);
            }
            
            var eventDetails = new SecurityEventDetailsDto( // REQ-7-001
                EventType: "AuthenticationAttempt",
                UserId: request.Username,
                Timestamp: DateTime.UtcNow,
                Outcome: "Attempt",
                Details: $"User '{request.Username}' attempting authentication via '{request.AuthType}'.",
                SourceIP: request.ClientIpAddress // Assuming AuthenticationRequestDto contains ClientIpAddress
            );
            await _auditLogService.LogEventAsync(eventDetails);

            try
            {
                // REQ-7-006: Orchestrates user authentication using configured identity providers
                AuthenticationResultDto authResult = await _identityProviderService.AuthenticateUserAsync(request);

                if (authResult.IsAuthenticated)
                {
                    eventDetails = eventDetails with { Outcome = "Success", UserId = authResult.UserId, Details = $"User '{authResult.UserName}' authenticated successfully. Roles: {string.Join(",", authResult.Roles ?? new List<string>())}" };
                    await _auditLogService.LogEventAsync(eventDetails); // REQ-7-001
                }
                else
                {
                    eventDetails = eventDetails with { Outcome = "Failure", Details = $"Authentication failed for user '{request.Username}': {authResult.ErrorMessage}" };
                    await _auditLogService.LogEventAsync(eventDetails); // REQ-7-001
                    // As per SDS: "Throws AuthenticationFailedException ... if the provider returns an unsuccessful result."
                    throw new AuthenticationFailedException(authResult.ErrorMessage ?? "Authentication provider returned failure.");
                }
                return authResult;
            }
            catch (AuthenticationFailedException) // Re-throw if already this type
            {
                throw;
            }
            catch (Exception ex)
            {
                var errorMsg = $"Critical error during authentication for user '{request.Username}': {ex.Message}";
                eventDetails = eventDetails with { Outcome = "Error", Details = errorMsg };
                await _auditLogService.LogEventAsync(eventDetails); // REQ-7-001
                // As per SDS: "Throws AuthenticationFailedException on critical errors before calling the provider" (or during).
                throw new AuthenticationFailedException(errorMsg, ex);
            }
        }
    }
}