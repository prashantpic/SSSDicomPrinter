using FluentValidation;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using System;

namespace TheSSS.DICOMViewer.Monitoring.Validators
{
    public class MonitoringOptionsValidator : AbstractValidator<MonitoringOptions>
    {
        public MonitoringOptionsValidator()
        {
            RuleFor(x => x.SystemHealthCheckInterval)
                .GreaterThan(TimeSpan.Zero)
                .WithMessage("SystemHealthCheckInterval must be a positive time span.");

            RuleFor(x => x.CriticalErrorLookbackPeriod)
                .GreaterThan(TimeSpan.Zero)
                .WithMessage("CriticalErrorLookbackPeriod must be a positive time span.");

            // IsMonitoringEnabled is a boolean, usually no validation needed unless specific requirements.
        }
    }
}