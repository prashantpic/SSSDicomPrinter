namespace TheSSS.DICOMViewer.Monitoring.Contracts;

/// <summary>
/// Represents the overall health status of the system or a component.
/// </summary>
public enum SystemStatus
{
    /// <summary>
    /// Indicates that the system or component is operating normally.
    /// </summary>
    Healthy,

    /// <summary>
    /// Indicates that the system or component is experiencing non-critical issues that may require attention.
    /// </summary>
    Warning,

    /// <summary>
    /// Indicates that the system or component is experiencing critical issues and is not functioning correctly.
    /// </summary>
    Error,

    /// <summary>
    /// Indicates that the status of the system or component is unknown.
    /// </summary>
    Unknown
}