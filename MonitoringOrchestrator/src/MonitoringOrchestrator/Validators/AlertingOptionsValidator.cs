using FluentValidation;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using System.Linq;

namespace TheSSS.DICOMViewer.Monitoring.Validators;

public class AlertingOptionsValidator : AbstractValidator<AlertingOptions>
{
    public AlertingOptionsValidator()
    {
        RuleFor(x => x.Rules).NotNull();
        RuleForEach(x => x.Rules).SetValidator(new AlertRuleValidator());
        
        RuleFor(x => x.Channels).NotNull();
        RuleForEach(x => x.Channels).SetValidator(new AlertChannelSettingValidator());
        
        RuleFor(x => x.Throttling).NotNull().SetValidator(new ThrottlingOptionsValidator());
        RuleFor(x => x.Deduplication).NotNull().SetValidator(new DeduplicationOptionsValidator());
    }
}

public class AlertRuleValidator : AbstractValidator<AlertRule>
{
    private static readonly string[] ValidComparisonOperators = { "GreaterThan", "LessThan", "EqualTo", "NotEqualTo" };
    private static readonly string[] ValidSeverities = { "Info", "Warning", "Critical" };

    public AlertRuleValidator()
    {
        RuleFor(x => x.RuleName).NotEmpty();
        RuleFor(x => x.MetricType).NotEmpty();
        RuleFor(x => x.ComparisonOperator)
            .Must(op => ValidComparisonOperators.Contains(op))
            .WithMessage("Invalid comparison operator");
        
        RuleFor(x => x.Severity)
            .Must(sev => ValidSeverities.Contains(sev))
            .WithMessage("Invalid severity level");
        
        RuleFor(x => x.ConsecutiveFailuresToAlert)
            .GreaterThanOrEqualTo(1);
    }
}

public class AlertChannelSettingValidator : AbstractValidator<AlertChannelSetting>
{
    public AlertChannelSettingValidator()
    {
        RuleFor(x => x.ChannelType).NotEmpty();
        RuleFor(x => x.RecipientDetails)
            .NotEmpty()
            .When(x => x.ChannelType == "Email");
    }
}

public class ThrottlingOptionsValidator : AbstractValidator<ThrottlingOptions>
{
    public ThrottlingOptionsValidator()
    {
        RuleFor(x => x.DefaultThrottleWindow).GreaterThanOrEqualTo(TimeSpan.Zero);
        RuleFor(x => x.MaxAlertsPerWindow).GreaterThanOrEqualTo(0);
    }
}

public class DeduplicationOptionsValidator : AbstractValidator<DeduplicationOptions>
{
    public DeduplicationOptionsValidator()
    {
        RuleFor(x => x.DeduplicationWindow).GreaterThanOrEqualTo(TimeSpan.Zero);
    }
}