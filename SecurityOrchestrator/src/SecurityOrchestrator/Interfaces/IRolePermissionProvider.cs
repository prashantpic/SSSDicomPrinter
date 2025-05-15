using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Security.Interfaces;

/// <summary>
/// Defines the contract for retrieving role and permission information
/// necessary for RBAC enforcement by the SecurityPolicyEngine.
/// Implemented in Infrastructure (e.g., reading from database or configuration).
/// Requirements Addressed: REQ-7-005.
/// </summary>
public interface IRolePermissionProvider
{
    /// <summary>
    /// Retrieves all permissions directly assigned to a user or granted through their roles.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of permission strings.</returns>
    Task<IEnumerable<string>> GetUserPermissionsAsync(string userId);

    /// <summary>
    /// Retrieves all roles assigned to a user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of role name strings.</returns>
    Task<IEnumerable<string>> GetUserRolesAsync(string userId);
}