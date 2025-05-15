using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using FluentValidation.Results; // Assuming FluentValidation is used for errors

namespace TheSSS.DICOMViewer.Monitoring.Exceptions
{
    /// <summary>
    /// Exception thrown when monitoring or alerting configuration is invalid.
    /// </summary>
    [Serializable]
    public class ConfigurationValidationException : MonitoringOrchestratorException
    {
        /// <summary>
        /// Gets the collection of validation errors.
        /// </summary>
        public IEnumerable<ValidationFailure> Errors { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationValidationException"/> class.
        /// </summary>
        /// <param name="errors">The collection of validation errors.</param>
        public ConfigurationValidationException(IEnumerable<ValidationFailure> errors)
            : this(BuildErrorMessage(errors), errors)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationValidationException"/> class with a specified error message
        /// and a collection of validation errors.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="errors">The collection of validation errors.</param>
        public ConfigurationValidationException(string message, IEnumerable<ValidationFailure> errors)
            : base(message)
        {
            Errors = errors ?? Enumerable.Empty<ValidationFailure>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationValidationException"/> class with a specified error message,
        /// a reference to the inner exception that is the cause of this exception, and a collection of validation errors.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference if no inner exception is specified.</param>
        /// <param name="errors">The collection of validation errors.</param>
        public ConfigurationValidationException(string message, Exception innerException, IEnumerable<ValidationFailure> errors)
            : base(message, innerException)
        {
            Errors = errors ?? Enumerable.Empty<ValidationFailure>();
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationValidationException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected ConfigurationValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Errors = (info.GetValue("Errors", typeof(IEnumerable<ValidationFailure>)) as IEnumerable<ValidationFailure>) 
                     ?? Enumerable.Empty<ValidationFailure>();
        }

        /// <summary>
        /// Sets the <see cref="SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Errors", Errors, typeof(IEnumerable<ValidationFailure>));
        }
        
        private static string BuildErrorMessage(IEnumerable<ValidationFailure> errors)
        {
            var errorMessages = errors?.Select(e => $"{e.PropertyName}: {e.ErrorMessage}").ToArray() 
                                ?? Array.Empty<string>();
            return $"Configuration validation failed: {Environment.NewLine}{string.Join(Environment.NewLine, errorMessages)}";
        }
    }
}