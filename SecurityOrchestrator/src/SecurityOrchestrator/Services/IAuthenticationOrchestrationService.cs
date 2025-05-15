using System.Threading.Tasks;
using TheSSS.DICOMViewer.Security.DTOs;

namespace TheSSS.DICOMViewer.Security.Services
{
    /// <summary>
    /// Defines the contract for orchestrating user authentication processes,
    /// abstracting the specific identity provider interaction.
    /// REQ-7-006
    /// </summary>
    public interface IAuthenticationOrchestrationService
    {
        /// <summary>
        /// Authenticates a user based on the provided authentication request.
        /// </summary>
        /// <param name="request">The authentication request details.</param>
        /// <returns>An <see cref="AuthenticationResultDto"/> indicating the outcome of the authentication attempt.</returns>
        Task<AuthenticationResultDto> AuthenticateAsync(AuthenticationRequestDto request);
    }
}