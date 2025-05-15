using System.Threading.Tasks;
using TheSSS.DICOMViewer.Security.DTOs;

namespace TheSSS.DICOMViewer.Security.Services
{
    /// <summary>
    /// Defines the contract for performing authorization checks to determine
    /// if a user has the required permissions for an action.
    /// REQ-7-005
    /// </summary>
    public interface IAuthorizationService
    {
        /// <summary>
        /// Checks if a user is authorized to perform a specific action based on RBAC.
        /// </summary>
        /// <param name="request">The authorization request details.</param>
        /// <returns>An <see cref="AuthorizationResultDto"/> indicating whether the user is authorized and the reason if not.</returns>
        Task<AuthorizationResultDto> IsAuthorizedAsync(AuthorizationRequestDto request);
    }
}