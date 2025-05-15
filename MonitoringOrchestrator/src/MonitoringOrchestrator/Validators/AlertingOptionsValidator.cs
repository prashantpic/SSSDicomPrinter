using FluentValidation;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using TheSSS.DICOMViewer.Monitoring.Contracts; // For AlertSeverity
using System;
using System.Linq;

namespace TheSSS.DICOMViewer.Monitoring.Validators
{
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

            RuleFor(x => x.DefaultAlertSourceComponent)
                .NotEmpty()
                .WithMessage("DefaultAlertSourceComponent must not be empty.");
        }
    }

    public class AlertRuleValidator : AbstractValidator<AlertRule>
    {
        private static readonly string[] ValidComparisonOperators = {
            "GreaterThan", "LessThan", "EqualTo", "NotEqualTo", "Contains", "DoesNotContain"
        };
        private static readonly string[] ValidSeverities = Enum.GetNames(typeof(AlertSeverity));

        public AlertRuleValidator()
        {
            RuleFor(x => x.RuleName)
                .NotEmpty()
                .WithMessage("RuleName must not be empty.");

            RuleFor(x => x.MetricType)
                .NotEmpty()
                .WithMessage("MetricType must not be empty.");

            RuleFor(x => x.ComparisonOperator)
                .NotEmpty()
                .Must(op => ValidComparisonOperators.Contains(op))
                .WithMessage($"ComparisonOperator must be one of: {string.Join(", ", ValidComparisonOperators)}.");

            RuleFor(x => x.Severity)
                .NotEmpty()
                .Must(sev => ValidSeverities.Any(s => s.Equals(sev, StringComparison.OrdinalIgnoreCase)))
                .WithMessage($"Severity must be one of: {string.Join(", ", ValidSeverities)}.");

            RuleFor(x => x.ConsecutiveFailuresToAlert)
                .GreaterThanOrEqualTo(1)
                .WithMessage("ConsecutiveFailuresToAlert must be 1 or greater.");

            // Conditional validation: ThresholdValue is required for numeric comparisons
            When(x => x.ComparisonOperator == "GreaterThan" || x.ComparisonOperator == "LessThan", () =>
            {
                RuleFor(x => x.ThresholdValue)
                    .NotNull()
                    .WithMessage("ThresholdValue must be provided for GreaterThan/LessThan comparisons.");
            });
            // ExpectedStatus is required for string/status comparisons
            When(x => x.ComparisonOperator == "EqualTo" || x.ComparisonOperator == "NotEqualTo" || x.ComparisonOperator == "Contains" || x.ComparisonOperator == "DoesNotContain", () =>
            {
                // If ThresholdValue is not set, ExpectedStatus should be.
                // This logic can be tricky if a metric type can be either numeric or string based on context.
                // For now, if ThresholdValue is null, we assume ExpectedStatus is used.
                RuleFor(x => x.ExpectedStatus)
                    .NotEmpty()
                    .When(x => x.ThresholdValue == null)
                    .WithMessage("ExpectedStatus must be provided for string/status comparisons if ThresholdValue is not set.");
            });

            RuleFor(x => x.ThrottleWindowOverride)
                .Must(t => t == null || t.Value >= TimeSpan.Zero)
                .WithMessage("ThrottleWindowOverride must be a non-negative time span if provided.");

            RuleFor(x => x.DeduplicationWindowOverride)
                .Must(t => t == null || t.Value >= TimeSpan.Zero)
                .WithMessage("DeduplicationWindowOverride must be a non-negative time span if provided.");
        }
    }

    public class AlertChannelSettingValidator : AbstractValidator<AlertChannelSetting>
    {
        public AlertChannelSettingValidator()
        {
            RuleFor(x => x.ChannelType)
                .NotEmpty()
                .WithMessage("ChannelType must not be empty.");

            When(x => x.ChannelType.Equals("Email", StringComparison.OrdinalIgnoreCase), () =>
            {
                RuleFor(x => x.RecipientDetails)
                    .NotEmpty()
                    .WithMessage("RecipientDetails must not be empty for Email channel.")
                    .ForEach(recipientList =>
                    {
                        recipientList.Must(email => !string.IsNullOrWhiteSpace(email) && email.Contains('@')) // Basic email validation
                                   .WithMessage("Invalid email format in RecipientDetails for Email channel.");
                    });
            });

            RuleFor(x => x.Severities)
                .ForEach(severityList =>
                {
                    severityList.Must(sev => AlertRuleValidator.ValidSeverities.Any(s => s.Equals(sev, StringComparison.OrdinalIgnoreCase)))
                                .WithMessage($"Invalid severity value in Severities list. Must be one of: {string.Join(", ", AlertRuleValidator.ValidSeverities)}.");
                });
        }
    }

    public class ThrottlingOptionsValidator : AbstractValidator<ThrottlingOptions>
    {
        public ThrottlingOptionsValidator()
        {
            RuleFor(x => x.DefaultThrottleWindow)
                .GreaterThanOrEqualTo(TimeSpan.Zero)
                .WithMessage("DefaultThrottleWindow must be a non-negative time span.");

            RuleFor(x => x.MaxAlertsPerWindow)
                .GreaterThanOrEqualTo(0)
                .WithMessage("MaxAlertsPerWindow must be 0 or greater.");
        }
    }

    public class DeduplicationOptionsValidator : AbstractValidator<DeduplicationOptions>
    {
        public DeduplicationOptionsValidator()
        {
            RuleFor(x => x.DeduplicationWindow)
                .GreaterThanOrEqualTo(TimeSpan.Zero)
                .WithMessage("DeduplicationWindow must be a non-negative time span.");
        }
    }
}