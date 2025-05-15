using System;

namespace TheSSS.DICOMViewer.Monitoring.Exceptions;

/// <summary>
/// Exception thrown when a health data source is unavailable or fails to provide data.
/// </summary>
[Serializable]
public class DataSourceUnavailableException : MonitoringOrchestratorException
{
    /// <summary>
    /// Identifier or name of the data source that is unavailable.
    /// </summary>
    public string DataSourceName { get; }

    public DataSourceUnavailableException(string dataSourceName)
        : this(dataSourceName, $"Health data source '{dataSourceName}' is unavailable.") { }

    public DataSourceUnavailableException(string dataSourceName, string message)
        : base(message)
    {
        DataSourceName = dataSourceName;
    }

    public DataSourceUnavailableException(string dataSourceName, string message, Exception inner)
        : base(message, inner)
    {
        DataSourceName = dataSourceName;
    }

    protected DataSourceUnavailableException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context)
    {
        DataSourceName = info.GetString(nameof(DataSourceName)) ?? "Unknown";
    }

    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(DataSourceName), DataSourceName);
    }
}