namespace TheSSS.DICOMViewer.Monitoring.Exceptions;

[System.Serializable]
public class MonitoringOrchestratorException : System.Exception
{
    public MonitoringOrchestratorException() { }
    public MonitoringOrchestratorException(string message) : base(message) { }
    public MonitoringOrchestratorException(string message, Exception inner) : base(message, inner) { }
    protected MonitoringOrchestratorException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}