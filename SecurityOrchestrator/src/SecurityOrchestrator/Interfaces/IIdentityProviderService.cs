using System.Threading.Tasks;
using TheSSS.DICOMViewer.Security.DTOs;

namespace TheSSS.DICOMViewer.Security.Interfaces;

/// <summary>
/// Defines the contract for abstracting user authentication mechanisms,
/// whether local or external (e.g., Windows Authentication).
/// Implemented in Infrastructure (e.g., for Windows Auth, database user store).
/// Requirements Addressed: REQ-7-006.
/// </summary>
public interface IIdentityProviderService
{
    /// <summary>
    /// Authenticates a user based on the provided request.
    /// </summary>
    /// <param name="request">The authentication request details.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the authentication result.</returns>
    Task<AuthenticationResultDto> AuthenticateUserAsync(AuthenticationRequestDto request);
}