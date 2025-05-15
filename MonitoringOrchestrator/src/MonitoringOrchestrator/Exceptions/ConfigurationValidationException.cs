using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TheSSS.DICOMViewer.Monitoring.Exceptions;

[Serializable]
public class ConfigurationValidationException : MonitoringOrchestratorException
{
    public IEnumerable<string> ValidationErrors { get; }

    public ConfigurationValidationException(string message, IEnumerable<string> errors) 
        : base(message) => ValidationErrors = errors;

    public ConfigurationValidationException(string message, IEnumerable<string> errors, Exception inner) 
        : base(message, inner) => ValidationErrors = errors;

    protected ConfigurationValidationException(SerializationInfo info, StreamingContext context) 
        : base(info, context) 
        => ValidationErrors = (IEnumerable<string>)info.GetValue(nameof(ValidationErrors), typeof(IEnumerable<string>))!;

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(ValidationErrors), ValidationErrors);
    }
}