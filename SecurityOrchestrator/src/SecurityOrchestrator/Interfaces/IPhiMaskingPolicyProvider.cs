using System.Threading.Tasks;
using TheSSS.DICOMViewer.Security.DTOs;

namespace TheSSS.DICOMViewer.Security.Interfaces;

/// <summary>
/// Defines the contract for obtaining PHI (Protected Health Information) masking rules
/// for specific contexts, e.g., log messages or audit trail details.
/// Implemented in Infrastructure or Cross-Cutting (e.g., reading from configuration).
/// Requirements Addressed: REQ-7-004.
/// </summary>
public interface IPhiMaskingPolicyProvider
{
    /// <summary>
    /// Retrieves PHI masking rules for a given context.
    /// </summary>
    /// <param name="context">The context for which to retrieve masking rules (e.g., "AuditLog.Authentication", "LogMessage.PatientDetails").</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the PHI masking rules.</returns>
    Task<PhiMaskingRulesDto> GetPhiMaskingRulesAsync(string context);
}