using System;
using System.Runtime.Serialization;

namespace TheSSS.DICOMViewer.Monitoring.Exceptions
{
    /// <summary>
    /// Exception thrown for errors occurring within the alert dispatching system.
    /// </summary>
    [Serializable]
    public class AlertingSystemException : MonitoringOrchestratorException
    {
        /// <summary>
        /// Gets the type of the channel that failed, if applicable.
        /// </summary>
        public string? ChannelType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlertingSystemException"/> class.
        /// </summary>
        public AlertingSystemException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlertingSystemException"/> class with a specified error message
        /// and an optional channel type.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="channelType">The type of the channel that failed.</param>
        public AlertingSystemException(string message, string? channelType = null)
            : base(message)
        {
            ChannelType = channelType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AlertingSystemException"/> class with a specified error message,
        /// a reference to the inner exception that is the cause of this exception, and an optional channel type.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        /// <param name="channelType">The type of the channel that failed.</param>
        public AlertingSystemException(string message, Exception innerException, string? channelType = null)
            : base(message, innerException)
        {
            ChannelType = channelType;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="AlertingSystemException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected AlertingSystemException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ChannelType = info.GetString("ChannelType");
        }

        /// <summary>
        /// Sets the <see cref="SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("ChannelType", ChannelType);
        }
    }
}