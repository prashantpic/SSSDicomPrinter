using System;
using System.Runtime.Serialization;
using TheSSS.DICOMViewer.Monitoring.Contracts;

namespace TheSSS.DICOMViewer.Monitoring.Exceptions;

[Serializable]
public class AlertingSystemException : MonitoringOrchestratorException
{
    public string ChannelType { get; }
    public NotificationPayloadDto? Payload { get; }

    public AlertingSystemException(string channelType, string message) : base(message) 
        => ChannelType = channelType;

    public AlertingSystemException(string channelType, string message, Exception inner) 
        : base(message, inner) => ChannelType = channelType;

    public AlertingSystemException(string channelType, string message, NotificationPayloadDto payload, Exception inner)
        : base(message, inner) => (ChannelType, Payload) = (channelType, payload);

    protected AlertingSystemException(SerializationInfo info, StreamingContext context) 
        : base(info, context)
    {
        ChannelType = info.GetString(nameof(ChannelType)) ?? "Unknown";
        Payload = (NotificationPayloadDto?)info.GetValue(nameof(Payload), typeof(NotificationPayloadDto));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(ChannelType), ChannelType);
        info.AddValue(nameof(Payload), Payload, typeof(NotificationPayloadDto));
    }
}