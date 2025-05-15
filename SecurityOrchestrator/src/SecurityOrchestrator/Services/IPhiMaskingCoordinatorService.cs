using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Security.Services
{
    /// <summary>
    /// Defines the contract for coordinating the application of PHI masking rules,
    /// especially for logging and auditing.
    /// REQ-7-004
    /// </summary>
    public interface IPhiMaskingCoordinatorService
    {
        /// <summary>
        /// Applies PHI masking rules to a log message string.
        /// </summary>
        /// <param name="originalMessage">The original log message.</param>
        /// <param name="context">The context for which masking rules should be applied (e.g., "GeneralLog", "AuditDetail").</param>
        /// <returns>The masked log message string.</returns>
        Task<string> ApplyMaskingToLogMessageAsync(string originalMessage, string context);

        /// <summary>
        /// Converts an object to its string representation and applies PHI masking rules suitable for auditing.
        /// </summary>
        /// <param name="detailsObject">The object containing details that might include PHI.</param>
        /// <param name="eventType">The type of event, used as context to retrieve appropriate masking rules.</param>
        /// <returns>A masked string representation of the detailsObject suitable for audit logs.</returns>
        Task<string> GetMaskedDetailsForAuditAsync(object detailsObject, string eventType);
    }
}