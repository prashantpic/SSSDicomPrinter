using FluentValidation;
using TheSSS.DICOMViewer.Monitoring.Configuration;

namespace TheSSS.DICOMViewer.Monitoring.Validators;

public class AlertingOptionsValidator : AbstractValidator<AlertingOptions>
{
    public AlertingOptionsValidator()
    {
        RuleFor(x => x.Rules)
            .NotNull().WithMessage("Alerting rules cannot be null.")
            .Must(rules => rules == null || rules.All(rule => rule != null)).WithMessage("Alerting rules list contains null entries.");

        RuleForEach(x => x.Rules).SetValidator(new AlertRuleValidator());

        RuleFor(x => x.Channels)
            .NotNull().WithMessage("Alerting channels cannot be null.")
             .Must(channels => channels == null || channels.All(channel => channel != null)).WithMessage("Alerting channels list contains null entries.");

        RuleForEach(x => x.Channels).SetValidator(new AlertChannelSettingValidator());

        RuleFor(x => x.Throttling).NotNull().WithMessage("Throttling options cannot be null.");
        RuleFor(x => x.Throttling).SetValidator(new ThrottlingOptionsValidator());

        RuleFor(x => x.Deduplication).NotNull().WithMessage("Deduplication options cannot be null.");
        RuleFor(x => x.Deduplication).SetValidator(new DeduplicationOptionsValidator());
    }
}

public class AlertRuleValidator : AbstractValidator<AlertRule>
{
    public AlertRuleValidator()
    {
        RuleFor(x => x.RuleName).NotEmpty().WithMessage("Alert rule name cannot be empty.");
        RuleFor(x => x.MetricType).NotEmpty().WithMessage("Alert rule metric type cannot be empty.");
        RuleFor(x => x.ComparisonOperator).NotEmpty().WithMessage("Alert rule comparison operator cannot be empty.");
        RuleFor(x => x.Severity).NotEmpty().WithMessage("Alert rule severity cannot be empty.");

        // Basic validation for ThresholdValue - depends on MetricType, might need custom rule
        RuleFor(x => x.ThresholdValue)
            .GreaterThanOrEqualTo(0).When(x => IsNumericMetric(x.MetricType)).WithMessage("Threshold value must be non-negative for numeric metrics.");

        // Basic validation for ComparisonOperator - check if it's a known operator
        RuleFor(x => x.ComparisonOperator)
            .Must(BeAValidComparisonOperator).WithMessage("Invalid comparison operator. Supported: GreaterThan, GreaterThanOrEqualTo, LessThan, LessThanOrEqualTo, EqualTo, NotEqualTo.");

        // ConsecutiveFailuresToAlert validation
        RuleFor(x => x.ConsecutiveFailuresToAlert)
            .GreaterThanOrEqualTo(1).When(x => RequiresConsecutiveFailureCheck(x.MetricType)).WithMessage("Consecutive failures to alert must be at least 1 for this metric type.");
    }

    private bool IsNumericMetric(string metricType)
    {
        // Define metric types that use numeric thresholds
        return metricType switch
        {
            "StorageUsagePercent" => true,
            "DatabaseConnectivity" => true, // Can be checked via IsConnected=false (0) or LatencyMs
            "PacsConnectivity" => true, // Can be checked via count of failed nodes
            "LicenseStatus" => true, // Can be checked via DaysUntilExpiry
            "CriticalErrorCount" => true,
            "AutomatedTaskFailureCount" => true,
            _ => false
        };
    }

     private bool RequiresConsecutiveFailureCheck(string metricType)
    {
        // Define metric types that typically use consecutive failure logic (e.g., external service checks)
        return metricType switch
        {
            "PacsConnectivity" => true,
            "DatabaseConnectivity" => true,
            // Add others if applicable
            _ => false
        };
    }


    private bool BeAValidComparisonOperator(string operatorValue)
    {
        return operatorValue == "GreaterThan" ||
               operatorValue == "GreaterThanOrEqualTo" ||
               operatorValue == "LessThan" ||
               operatorValue == "LessThanOrEqualTo" ||
               operatorValue == "EqualTo" ||
               operatorValue == "NotEqualTo";
    }
}

public class AlertChannelSettingValidator : AbstractValidator<AlertChannelSetting>
{
    public AlertChannelSettingValidator()
    {
        RuleFor(x => x.ChannelType).NotEmpty().WithMessage("Alert channel type cannot be empty.");
        RuleFor(x => x.ChannelType)
             .Must(BeAValidChannelType).WithMessage("Invalid channel type. Supported: Email, UI, AuditLog.");

        RuleFor(x => x.RecipientEmailAddresses)
            .NotNull().When(x => x.ChannelType == "Email").WithMessage("Recipient email addresses must be provided for Email channel.");

        RuleForEach(x => x.RecipientEmailAddresses)
            .NotEmpty().WithMessage("Recipient email address cannot be empty.")
            .EmailAddress().WithMessage("Invalid email address format.")
            .When(x => x.ChannelType == "Email");
    }

     private bool BeAValidChannelType(string channelType)
    {
        return channelType == "Email" ||
               channelType == "UI" ||
               channelType == "AuditLog";
    }
}

public class ThrottlingOptionsValidator : AbstractValidator<ThrottlingOptions>
{
    public ThrottlingOptionsValidator()
    {
        When(x => x.IsEnabled, () =>
        {
            RuleFor(x => x.DefaultThrottleWindow)
                .GreaterThan(TimeSpan.Zero).WithMessage("Default throttle window must be a positive time span.");
            RuleFor(x => x.MaxAlertsPerWindow)
                .GreaterThanOrEqualTo(1).WithMessage("Max alerts per window must be at least 1.");
        });
    }
}

public class DeduplicationOptionsValidator : AbstractValidator<DeduplicationOptions>
{
    public DeduplicationOptionsValidator()
    {
         When(x => x.IsEnabled, () =>
        {
            RuleFor(x => x.DeduplicationWindow)
                .GreaterThan(TimeSpan.Zero).WithMessage("Deduplication window must be a positive time span.");
        });
    }
}