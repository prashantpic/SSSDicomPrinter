using System;
using System.Text.Json;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Security.DTOs;
using TheSSS.DICOMViewer.Security.Exceptions;
using TheSSS.DICOMViewer.Security.Interfaces;

namespace TheSSS.DICOMViewer.Security.Services
{
    public class PhiMaskingCoordinatorService : IPhiMaskingCoordinatorService
    {
        private readonly IPhiMaskingPolicyProvider _phiMaskingPolicyProvider;
        private readonly IAuditLogService _auditLogService;

        public PhiMaskingCoordinatorService(
            IPhiMaskingPolicyProvider phiMaskingPolicyProvider,
            IAuditLogService auditLogService)
        {
            _phiMaskingPolicyProvider = phiMaskingPolicyProvider ?? throw new ArgumentNullException(nameof(phiMaskingPolicyProvider));
            _auditLogService = auditLogService ?? throw new ArgumentNullException(nameof(auditLogService));
        }

        public async Task<string> ApplyMaskingToLogMessageAsync(string originalMessage, string context) // REQ-7-004
        {
            if (string.IsNullOrEmpty(originalMessage))
            {
                return originalMessage;
            }

            var eventDetails = new SecurityEventDetailsDto( // REQ-7-001
                EventType: "PhiMaskingAttempt",
                UserId: "SYSTEM", // Masking is often a system operation
                Timestamp: DateTime.UtcNow,
                Outcome: "Attempt",
                Details: $"Attempting PHI masking for log message. Context: {context}, Original Length: {originalMessage.Length}.",
                SourceIP: null
            );
            // Do not log originalMessage here to avoid defeating the purpose.

            try
            {
                PhiMaskingRulesDto maskingRules = await _phiMaskingPolicyProvider.GetPhiMaskingRulesAsync(context);
                string maskedMessage = ApplyRules(originalMessage, maskingRules);
                
                eventDetails = eventDetails with { Outcome = "Success", Details = $"PHI masking applied to log message. Context: {context}, Masked Length: {maskedMessage.Length}."};
                // Log the masking operation itself, not the content being masked.
                await _auditLogService.LogEventAsync(eventDetails); // REQ-7-001

                return maskedMessage;
            }
            catch (Exception ex)
            {
                var errorMsg = $"Error during PHI masking for log message context '{context}': {ex.Message}";
                eventDetails = eventDetails with { Outcome = "Error", Details = errorMsg };
                await _auditLogService.LogEventAsync(eventDetails); // REQ-7-001
                // Log original message to a secure/debug log if absolutely necessary and policy allows,
                // otherwise, return original message or throw.
                // SDS: Throws PhiMaskingException on failure.
                throw new PhiMaskingException(errorMsg, ex);
            }
        }

        public async Task<string> GetMaskedDetailsForAuditAsync(object detailsObject, string eventType) // REQ-7-004
        {
            if (detailsObject == null)
            {
                return string.Empty;
            }

            string originalDetailsString;
            try
            {
                originalDetailsString = JsonSerializer.Serialize(detailsObject, new JsonSerializerOptions { WriteIndented = false });
            }
            catch (Exception ex)
            {
                // Fallback if serialization fails
                originalDetailsString = detailsObject.ToString() ?? "Error serializing detailsObject";
                 await _auditLogService.LogEventAsync(new SecurityEventDetailsDto( // REQ-7-001
                    EventType: "PhiMaskingFailure",
                    UserId: "SYSTEM",
                    Timestamp: DateTime.UtcNow,
                    Outcome: "Error",
                    Details: $"Failed to serialize detailsObject for PHI masking. EventType: {eventType}. Error: {ex.Message}",
                    SourceIP: null
                ));
                // Depending on policy, either throw or return a placeholder. SDS implies throw on failure.
                throw new PhiMaskingException($"Failed to serialize detailsObject for event type '{eventType}'.", ex);
            }
            
            var eventDetails = new SecurityEventDetailsDto( // REQ-7-001
                EventType: "PhiMaskingAuditAttempt",
                UserId: "SYSTEM",
                Timestamp: DateTime.UtcNow,
                Outcome: "Attempt",
                Details: $"Attempting PHI masking for audit details. EventType: {eventType}, Original Length: {originalDetailsString.Length}.",
                SourceIP: null
            );

            try
            {
                PhiMaskingRulesDto maskingRules = await _phiMaskingPolicyProvider.GetPhiMaskingRulesAsync(eventType); // Use eventType as context
                string maskedDetailsString = ApplyRules(originalDetailsString, maskingRules);
                
                eventDetails = eventDetails with { Outcome = "Success", Details = $"PHI masking applied for audit details. EventType: {eventType}, Masked Length: {maskedDetailsString.Length}."};
                await _auditLogService.LogEventAsync(eventDetails); // REQ-7-001

                return maskedDetailsString;
            }
            catch (Exception ex)
            {
                var errorMsg = $"Error during PHI masking for audit details event type '{eventType}': {ex.Message}";
                eventDetails = eventDetails with { Outcome = "Error", Details = errorMsg };
                await _auditLogService.LogEventAsync(eventDetails); // REQ-7-001
                throw new PhiMaskingException(errorMsg, ex);
            }
        }

        private string ApplyRules(string input, PhiMaskingRulesDto? rulesDto)
        {
            if (rulesDto == null || rulesDto.Rules == null || rulesDto.Rules.Count == 0)
            {
                return input;
            }

            string maskedInput = input;
            // Placeholder for actual masking logic.
            // In a real implementation, this would involve more sophisticated pattern matching (e.g., Regex)
            // and replacement strategies based on the rules.
            // The PhiMaskingRulesDto is IReadOnlyDictionary<string, string> - key is pattern, value is replacement.
            foreach (var rule in rulesDto.Rules)
            {
                // This is a very basic example. Real masking should use regex for keys if they are patterns.
                // For simplicity, direct string replacement is shown.
                // If rule.Key is a field name in a JSON string, more complex parsing/replacement is needed.
                // For now, assume rule.Key is a literal string to be found and rule.Value is its replacement.
                maskedInput = maskedInput.Replace(rule.Key, rule.Value);
            }
            return maskedInput;
        }
    }
}