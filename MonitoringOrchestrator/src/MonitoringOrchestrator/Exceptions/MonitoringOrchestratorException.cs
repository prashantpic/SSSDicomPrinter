using System;
using System.Runtime.Serialization;

namespace TheSSS.DICOMViewer.Monitoring.Exceptions;

/// <summary>
/// Base custom exception for the MonitoringOrchestrator service.
/// </summary>
[Serializable]
public class MonitoringOrchestratorException : Exception
{
    public MonitoringOrchestratorException() { }
    public MonitoringOrchestratorException(string message) : base(message) { }
    public MonitoringOrchestratorException(string message, Exception inner) : base(message, inner) { }

    protected MonitoringOrchestratorException(SerializationInfo info, StreamingContext context)
        : base(info, context) { }
}