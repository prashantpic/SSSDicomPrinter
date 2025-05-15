using FluentValidation;
using TheSSS.DICOMViewer.Monitoring.Configuration;

namespace TheSSS.DICOMViewer.Monitoring.Validators;

public class MonitoringOptionsValidator : AbstractValidator<MonitoringOptions>
{
    public MonitoringOptionsValidator()
    {
        RuleFor(x => x.SystemHealthCheckInterval)
            .GreaterThan(TimeSpan.Zero).WithMessage("System health check interval must be a positive time span.");

        // Add rules for other MonitoringOptions properties if any
    }
}