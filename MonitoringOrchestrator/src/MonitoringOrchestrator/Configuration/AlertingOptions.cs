using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheSSS.DICOMViewer.Monitoring.Configuration;

/// <summary>
/// POCO class for alerting configurations, including rules and channel settings, bindable from configuration.
/// </summary>
public class AlertingOptions
{
    /// <summary>
    /// Global flag to enable or disable alerting.
    /// If false, no alerts will be dispatched even if rules are triggered.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// List of defined alert rules. This list can be empty.
    /// </summary>
    [Required(ErrorMessage = "Rules list cannot be null, but can be empty.")]
    public List<AlertRule> Rules { get; set; } = new List<AlertRule>();

    /// <summary>
    /// List of configured alerting channels and their settings. This list can be empty.
    /// </summary>
    [Required(ErrorMessage = "Channels list cannot be null, but can be empty.")]
    public List<AlertChannelSetting> Channels { get; set; } = new List<AlertChannelSetting>();

    /// <summary>
    /// Global throttling options.
    /// </summary>
    [Required(ErrorMessage = "Throttling options are required.")]
    [ValidateObject] // Ensures nested validation if ThrottlingOptions has its own [Required] etc.
    public ThrottlingOptions Throttling { get; set; } = new ThrottlingOptions();

    /// <summary>
    /// Global deduplication options.
    /// </summary>
    [Required(ErrorMessage = "Deduplication options are required.")]
    [ValidateObject] // Ensures nested validation
    public DeduplicationOptions Deduplication { get; set; } = new DeduplicationOptions();
}


// Simple attribute to mark nested objects for validation by ValidateDataAnnotations.
// This allows standard .NET validation attributes on child objects to be checked.
// Requires the use of services.AddOptions<TOptions>().ValidateDataAnnotations().ValidateOnStart();
// and potentially a custom validation extension like the one in MonitoringServiceRegistrar
// if ValidateObject itself needs to trigger FluentValidation or complex logic.
// For simple [Required] on child properties, this standard approach is often enough.
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public sealed class ValidateObjectAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value == null)
        {
            return ValidationResult.Success; // Or an error if the object itself is required. [Required] handles that.
        }

        var results = new List<ValidationResult>();
        var context = new ValidationContext(value, validationContext.ServiceProvider, validationContext.Items);

        bool isValid = Validator.TryValidateObject(value, context, results, true);

        if (!isValid)
        {
            var compositeResults = new CompositeValidationResult($"Validation failed for {validationContext.DisplayName}.");
            results.ForEach(compositeResults.AddResult);
            return compositeResults;
        }

        return ValidationResult.Success;
    }

    private class CompositeValidationResult : ValidationResult
    {
        public List<ValidationResult> Results { get; } = new List<ValidationResult>();

        public CompositeValidationResult(string errorMessage) : base(errorMessage) { }
        public CompositeValidationResult(string errorMessage, IEnumerable<string>? memberNames) : base(errorMessage, memberNames) { }
        protected CompositeValidationResult(ValidationResult validationResult) : base(validationResult) { }


        public void AddResult(ValidationResult validationResult)
        {
            Results.Add(validationResult);
        }
    }
}