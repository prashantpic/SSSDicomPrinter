using FluentValidation;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using System;

namespace TheSSS.DICOMViewer.Monitoring.Validators
{
    /// <summary>
    /// Validator for the <see cref="MonitoringOptions"/> configuration class.
    /// Ensures that global monitoring settings, such as check intervals, are valid.
    /// </summary>
    public class MonitoringOptionsValidator : AbstractValidator<MonitoringOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MonitoringOptionsValidator"/> class.
        /// </summary>
        public MonitoringOptionsValidator()
        {
            RuleFor(options => options.SystemHealthCheckInterval)
                .GreaterThan(TimeSpan.Zero)
                .WithMessage("System health check interval must be a positive time span.");
            
            // Assuming MonitoringOptions now includes SystemErrorLookbackWindow
            RuleFor(options => options.SystemErrorLookbackWindow)
                .GreaterThan(TimeSpan.Zero)
                .WithMessage("System error lookback window must be a positive time span.");
        }
    }
}