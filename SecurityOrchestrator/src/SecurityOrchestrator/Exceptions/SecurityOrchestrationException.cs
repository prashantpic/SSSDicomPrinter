using System;

namespace TheSSS.DICOMViewer.Security.Exceptions;

/// <summary>
/// Base exception for all errors originating in the SecurityOrchestrator module.
/// This allows for centralized error handling of security-related issues.
/// Requirements Addressed: REQ-7-001, REQ-7-005, REQ-LDM-LIC-002, REQ-7-017 (as base for more specific exceptions).
/// </summary>
[Serializable]
public class SecurityOrchestrationException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SecurityOrchestrationException"/> class.
    /// </summary>
    public SecurityOrchestrationException()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SecurityOrchestrationException"/> class
    /// with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public SecurityOrchestrationException(string message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SecurityOrchestrationException"/> class
    /// with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
    public SecurityOrchestrationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}