using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation.Results;

namespace TheSSS.DICOMViewer.Monitoring.Exceptions;

/// <summary>
/// Exception thrown when monitoring or alerting configuration is invalid.
/// </summary>
[Serializable]
public class ConfigurationValidationException : MonitoringOrchestratorException
{
    /// <summary>
    /// Details of the validation failures.
    /// </summary>
    public IEnumerable<ValidationFailure> Errors { get; }

    public ConfigurationValidationException(IEnumerable<ValidationFailure> errors)
        : base("Monitoring or alerting configuration validation failed.")
    {
        Errors = errors?.ToList() ?? new List<ValidationFailure>();
    }

     public ConfigurationValidationException(IEnumerable<ValidationFailure> errors, string message)
        : base(message)
    {
        Errors = errors?.ToList() ?? new List<ValidationFailure>();
    }

    public ConfigurationValidationException(IEnumerable<ValidationFailure> errors, string message, Exception inner)
        : base(message, inner)
    {
         Errors = errors?.ToList() ?? new List<ValidationFailure>();
    }

    protected ConfigurationValidationException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context)
    {
        Errors = (IEnumerable<ValidationFailure>?)info.GetValue(nameof(Errors), typeof(IEnumerable<ValidationFailure>)) ?? new List<ValidationFailure>();
    }

    public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue(nameof(Errors), Errors);
    }
}