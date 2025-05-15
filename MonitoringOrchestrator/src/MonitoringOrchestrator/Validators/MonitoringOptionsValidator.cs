using FluentValidation;
using TheSSS.DICOMViewer.Monitoring.Configuration;

namespace TheSSS.DICOMViewer.Monitoring.Validators;

public class MonitoringOptionsValidator : AbstractValidator<MonitoringOptions>
{
    public MonitoringOptionsValidator()
    {
        RuleFor(x => x.SystemHealthCheckInterval)
            .GreaterThan(TimeSpan.Zero)
            .WithMessage("Health check interval must be positive");

        RuleFor(x => x.CriticalErrorLookbackPeriod)
            .GreaterThan(TimeSpan.Zero)
            .WithMessage("Error lookback period must be positive");
    }
}