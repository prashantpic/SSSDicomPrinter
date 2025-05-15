using System;
using System.Runtime.Serialization;

namespace TheSSS.DICOMViewer.Monitoring.Exceptions
{
    /// <summary>
    /// Exception thrown when a health data source is unavailable or fails to provide data.
    /// </summary>
    [Serializable]
    public class DataSourceUnavailableException : MonitoringOrchestratorException
    {
        /// <summary>
        /// Gets the name of the data source that was unavailable.
        /// </summary>
        public string? DataSourceName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSourceUnavailableException"/> class.
        /// </summary>
        public DataSourceUnavailableException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSourceUnavailableException"/> class with a specified error message
        /// and an optional data source name.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="dataSourceName">The name of the data source that was unavailable.</param>
        public DataSourceUnavailableException(string message, string? dataSourceName = null)
            : base(message)
        {
            DataSourceName = dataSourceName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSourceUnavailableException"/> class with a specified error message,
        /// a reference to the inner exception that is the cause of this exception, and an optional data source name.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        /// <param name="dataSourceName">The name of the data source that was unavailable.</param>
        public DataSourceUnavailableException(string message, Exception innerException, string? dataSourceName = null)
            : base(message, innerException)
        {
            DataSourceName = dataSourceName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataSourceUnavailableException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected DataSourceUnavailableException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            DataSourceName = info.GetString("DataSourceName");
        }

        /// <summary>
        /// Sets the <see cref="SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("DataSourceName", DataSourceName);
        }
    }
}