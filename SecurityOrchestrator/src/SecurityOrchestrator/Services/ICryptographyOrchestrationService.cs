using System.Threading.Tasks;
using TheSSS.DICOMViewer.Security.DTOs;

namespace TheSSS.DICOMViewer.Security.Services
{
    /// <summary>
    /// Defines the contract for orchestrating cryptographic operations,
    /// such as encryption and decryption of sensitive data.
    /// REQ-7-017
    /// </summary>
    public interface ICryptographyOrchestrationService
    {
        /// <summary>
        /// Protects (encrypts) the provided sensitive data.
        /// </summary>
        /// <param name="request">The request containing data to protect and protection parameters.</param>
        /// <returns>A <see cref="SensitiveDataProtectionResultDto"/> containing the protected data.</returns>
        Task<SensitiveDataProtectionResultDto> ProtectDataAsync(SensitiveDataProtectionRequestDto request);

        /// <summary>
        /// Unprotects (decrypts) the provided sensitive data.
        /// </summary>
        /// <param name="request">The request containing data to unprotect and unprotection parameters.</param>
        /// <returns>A <see cref="SensitiveDataUnprotectionResultDto"/> containing the unprotected data.</returns>
        Task<SensitiveDataUnprotectionResultDto> UnprotectDataAsync(SensitiveDataUnprotectionRequestDto request);
    }
}