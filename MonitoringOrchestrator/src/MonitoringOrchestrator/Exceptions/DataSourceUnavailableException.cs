using System;
using System.Runtime.Serialization;

namespace TheSSS.DICOMViewer.Monitoring.Exceptions;

[Serializable]
public class DataSourceUnavailableException : MonitoringOrchestratorException
{
    public string DataSourceName { get; }

    public DataSourceUnavailableException(string dataSourceName) : base($"Data source '{dataSourceName}' unavailable") 
        => DataSourceName = dataSourceName;

    public DataSourceUnavailableException(string dataSourceName, string message) : base(message) 
        => DataSourceName = dataSourceName;

    public DataSourceUnavailableException(string dataSourceName, string message, Exception inner) 
        : base(message, inner) => DataSourceName = dataSourceName;

    protected DataSourceUnavailableException(SerializationInfo info, StreamingContext context) 
        : base(info, context) 
        => DataSourceName = info.GetString(nameof(DataSourceName)) ?? "Unknown";

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(DataSourceName), DataSourceName);
    }
}