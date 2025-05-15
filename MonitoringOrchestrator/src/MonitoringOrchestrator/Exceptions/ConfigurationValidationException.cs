using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace TheSSS.DICOMViewer.Monitoring.Exceptions
{
    [Serializable]
    public class ConfigurationValidationException : MonitoringOrchestratorException
    {
        public IEnumerable<string> ValidationErrors { get; }

        public ConfigurationValidationException(string message, IEnumerable<string> errors)
            : base(message)
        {
            ValidationErrors = errors?.ToList() ?? Enumerable.Empty<string>();
        }

        public ConfigurationValidationException(string message, IEnumerable<string> errors, Exception innerException)
            : base(message, innerException)
        {
            ValidationErrors = errors?.ToList() ?? Enumerable.Empty<string>();
        }

        protected ConfigurationValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ValidationErrors = (IEnumerable<string>?)info.GetValue(nameof(ValidationErrors), typeof(IEnumerable<string>))
                               ?? Enumerable.Empty<string>();
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(ValidationErrors), ValidationErrors, typeof(IEnumerable<string>));
        }

        public override string Message
        {
            get
            {
                var baseMessage = base.Message;
                if (ValidationErrors.Any())
                {
                    return $"{baseMessage} Validation Errors: {string.Join("; ", ValidationErrors)}";
                }
                return baseMessage;
            }
        }
    }
}