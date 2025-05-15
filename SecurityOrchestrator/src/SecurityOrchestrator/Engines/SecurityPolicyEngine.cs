using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Security.Interfaces;

namespace TheSSS.DICOMViewer.Security.Engines
{
    public class SecurityPolicyEngine
    {
        private readonly IRolePermissionProvider _rolePermissionProvider;

        public SecurityPolicyEngine(IRolePermissionProvider rolePermissionProvider)
        {
            _rolePermissionProvider = rolePermissionProvider ?? throw new ArgumentNullException(nameof(rolePermissionProvider));
        }

        // REQ-7-005: Core logic for RBAC enforcement.
        public async Task<bool> EvaluatePermissionAsync(string userId, string permission, string? resourceContext)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(permission))
            {
                // Basic validation
                return false;
            }

            try
            {
                // This is a placeholder for more complex policy evaluation.
                // Actual implementation would depend on how IRolePermissionProvider exposes roles and permissions.

                // Example 1: Get all effective permissions for the user and check if the requested permission is present.
                // This assumes IRolePermissionProvider has a method like GetUserEffectivePermissionsAsync.
                // var effectivePermissions = await _rolePermissionProvider.GetUserEffectivePermissionsAsync(userId, resourceContext);
                // return effectivePermissions?.Contains(permission) ?? false;

                // Example 2: More granular approach - get roles, then permissions for roles, and direct user permissions.
                var userRoles = await _rolePermissionProvider.GetUserRolesAsync(userId);
                if (userRoles != null)
                {
                    foreach (var role in userRoles)
                    {
                        var rolePermissions = await _rolePermissionProvider.GetPermissionsForRoleAsync(role.RoleName); // Assuming role DTO has RoleName
                        if (rolePermissions != null && rolePermissions.Any(p => string.Equals(p.PermissionName, permission, StringComparison.OrdinalIgnoreCase)))
                        {
                            // TODO: Consider resourceContext if permissions are context-specific
                            return true;
                        }
                    }
                }

                var directUserPermissions = await _rolePermissionProvider.GetDirectUserPermissionsAsync(userId);
                if (directUserPermissions != null && directUserPermissions.Any(p => string.Equals(p.PermissionName, permission, StringComparison.OrdinalIgnoreCase)))
                {
                     // TODO: Consider resourceContext if permissions are context-specific
                    return true;
                }
                
                // Placeholder: Simple hardcoded rule for demonstration if provider calls are not fully fleshed out.
                // if (userId == "admin" && permission == "ManageSystem") return true;
                // if (permission == "ViewPublicData") return true;


                // For now, returning false as a default if no specific permissions match.
                // The actual complex policy evaluation is outside the scope of this simplified generation.
                // This structure allows for plugging in real logic.
                return false; 
            }
            catch (Exception ex)
            {
                // Log error (ideally via a logger injected here or handled by the caller)
                Console.WriteLine($"Error evaluating permission for user {userId}, permission {permission}: {ex.Message}");
                // Depending on policy, either return false or re-throw as a specific policy engine exception.
                // Returning false is safer to prevent unintended access on error.
                return false;
            }
        }
    }
}