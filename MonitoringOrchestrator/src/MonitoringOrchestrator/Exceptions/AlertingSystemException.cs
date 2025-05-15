using System;

namespace TheSSS.DICOMViewer.Monitoring.Exceptions;

/// <summary>
/// Exception thrown for errors occurring within the alert dispatching system.
/// </summary>
[Serializable]
public class AlertingSystemException : MonitoringOrchestratorException
{
    /// <summary>
    /// The type of alerting channel that failed.
    /// </summary>
    public string ChannelType { get; } = string.Empty;

    /// <summary>
    /// The name of the triggered rule associated with the failed alert dispatch.
    /// </summary>
    public string? TriggeredRuleName { get; }


    public AlertingSystemException() { }

    public AlertingSystemException(string message) : base(message) { }

    public AlertingSystemException(string message, Exception inner) : base(message, inner) { }

     public AlertingSystemException(string channelType, string message)
        : base($"Alert dispatch failed for channel '{channelType}': {message}")
    {
        ChannelType = channelType;
    }

    public AlertingSystemException(string channelType, string triggeredRuleName, string message)
       : base($"Alert dispatch failed for channel '{channelType}' (Rule: '{triggeredRuleName}'): {message}")
   {
       ChannelType = channelType;
       TriggeredRuleName = triggeredRuleName;
   }

    public AlertingSystemException(string channelType, string triggeredRuleName, string message, Exception inner)
        : base($"Alert dispatch failed for channel '{channelType}' (Rule: '{triggeredRuleName}'): {message}", inner)
    {
        ChannelType = channelType;
        TriggeredRuleName = triggeredRuleName;
    }


    protected AlertingSystemException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context)
    {
        ChannelType = info.GetString(nameof(ChannelType)) ?? string.Empty;
        TriggeredRuleName = info.GetString(nameof(TriggeredRuleName));
    }

     public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(ChannelType), ChannelType);
        info.AddValue(nameof(TriggeredRuleName), TriggeredRuleName);
    }
}