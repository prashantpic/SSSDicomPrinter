namespace TheSSS.DICOMViewer.Monitoring.Exceptions;

using System;

[Serializable]
public class MonitoringOrchestratorException : Exception
{
    public MonitoringOrchestratorException() { }
    public MonitoringOrchestratorException(string message) : base(message) { }
    public MonitoringOrchestratorException(string message, Exception inner) : base(message, inner) { }
    protected MonitoringOrchestratorException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}