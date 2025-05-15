using System;

namespace TheSSS.DICOMViewer.Security.Exceptions
{
    /// <summary>
    /// Specific exception thrown when an authorization check fails,
    /// indicating a user lacks necessary permissions.
    /// REQ-7-005
    /// </summary>
    public class AuthorizationFailedException : SecurityOrchestrationException
    {
        public AuthorizationFailedException(string userId, string permission)
            : base($"User '{userId}' is not authorized for permission '{permission}'.")
        {
        }

        public AuthorizationFailedException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}