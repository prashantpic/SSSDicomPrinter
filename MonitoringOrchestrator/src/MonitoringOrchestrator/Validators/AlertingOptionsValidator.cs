using FluentValidation;
using TheSSS.DICOMViewer.Monitoring.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TheSSS.DICOMViewer.Monitoring.Validators
{
    /// <summary>
    /// Validator for the <see cref="AlertingOptions"/> configuration class.
    /// Ensures that alerting configurations are valid before use.
    /// </summary>
    public class AlertingOptionsValidator : AbstractValidator<AlertingOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AlertingOptionsValidator"/> class.
        /// </summary>
        public AlertingOptionsValidator()
        {
            RuleFor(options => options.Rules)
                .NotNull().WithMessage("Alert rules list cannot be null.")
                .ForEach(rule => rule.SetValidator(new AlertRuleValidator()));

            RuleFor(options => options.Channels)
                .NotNull().WithMessage("Alert channels list cannot be null.")
                .ForEach(channel => channel.SetValidator(new AlertChannelSettingValidator()));

            RuleFor(options => options.Throttling)
                .NotNull().WithMessage("Throttling options cannot be null.")
                .SetValidator(new ThrottlingOptionsValidator());

            RuleFor(options => options.Deduplication)
                .NotNull().WithMessage("Deduplication options cannot be null.")
                .SetValidator(new DeduplicationOptionsValidator());
        }
    }

    /// <summary>
    /// Validator for the <see cref="AlertRule"/> configuration class.
    /// </summary>
    public class AlertRuleValidator : AbstractValidator<AlertRule>
    {
        private static readonly HashSet<string> ValidComparisonOperators = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "GreaterThan", "LessThan", "EqualTo", "NotEqualTo", "GreaterThanOrEqualTo", "LessThanOrEqualTo"
        };

        private static readonly HashSet<string> ValidSeverities = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Information", "Warning", "Error", "Critical"
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="AlertRuleValidator"/> class.
        /// </summary>
        public AlertRuleValidator()
        {
            RuleFor(rule => rule.RuleName)
                .NotEmpty().WithMessage("Rule name cannot be empty.");

            RuleFor(rule => rule.MetricType)
                .NotEmpty().WithMessage("Metric type cannot be empty.");

            RuleFor(rule => rule.Severity)
                .NotEmpty().WithMessage("Severity cannot be empty.")
                .Must(severity => ValidSeverities.Contains(severity))
                .WithMessage(severity => $"Severity '{severity.Severity}' is not a valid. Must be one of: {string.Join(", ", ValidSeverities)}.");

            RuleFor(rule => rule.ComparisonOperator)
                .NotEmpty().WithMessage("Comparison operator cannot be empty.")
                .Must(op => ValidComparisonOperators.Contains(op))
                .WithMessage(rule => $"Comparison operator '{rule.ComparisonOperator}' is not valid. Must be one of: {string.Join(", ", ValidComparisonOperators)}.");

            RuleFor(rule => rule.ConsecutiveFailuresToAlert)
                .GreaterThanOrEqualTo(1).WithMessage("Consecutive failures to alert must be 1 or greater.");
            
            // ThresholdValue validation is highly dependent on MetricType, so generic validation is limited.
            // Specific validation might be needed elsewhere or if MetricType has known value ranges.
        }
    }

    /// <summary>
    /// Validator for the <see cref="AlertChannelSetting"/> configuration class.
    /// </summary>
    public class AlertChannelSettingValidator : AbstractValidator<AlertChannelSetting>
    {
        private static readonly HashSet<string> ValidChannelTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Email", "UI", "AuditLog"
        };
        
        private static readonly HashSet<string> ValidSeverities = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Information", "Warning", "Error", "Critical"
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="AlertChannelSettingValidator"/> class.
        /// </summary>
        public AlertChannelSettingValidator()
        {
            RuleFor(channel => channel.ChannelType)
                .NotEmpty().WithMessage("Channel type cannot be empty.")
                .Must(type => ValidChannelTypes.Contains(type))
                .WithMessage(channel => $"Channel type '{channel.ChannelType}' is not valid. Must be one of: {string.Join(", ", ValidChannelTypes)}.");

            When(channel => channel.IsEnabled && "Email".Equals(channel.ChannelType, StringComparison.OrdinalIgnoreCase), () =>
            {
                RuleFor(channel => channel.RecipientEmailAddresses)
                    .NotNull().WithMessage("Recipient email addresses cannot be null for an enabled Email channel.")
                    .NotEmpty().WithMessage("Recipient email addresses cannot be empty for an enabled Email channel.")
                    .ForEach(emailRule => emailRule.EmailAddress().WithMessage("Invalid email address format."));
            });

            When(channel => !string.IsNullOrEmpty(channel.MinimumSeverity), () =>
            {
                RuleFor(channel => channel.MinimumSeverity)
                    .Must(severity => ValidSeverities.Contains(severity!))
                    .WithMessage(channel => $"Minimum severity '{channel.MinimumSeverity}' is not a valid. Must be one of: {string.Join(", ", ValidSeverities)}.");
            });
        }
    }

    /// <summary>
    /// Validator for the <see cref="ThrottlingOptions"/> configuration class.
    /// </summary>
    public class ThrottlingOptionsValidator : AbstractValidator<ThrottlingOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThrottlingOptionsValidator"/> class.
        /// </summary>
        public ThrottlingOptionsValidator()
        {
            When(options => options.IsEnabled, () =>
            {
                RuleFor(options => options.DefaultThrottleWindow)
                    .GreaterThan(TimeSpan.Zero).WithMessage("Default throttle window must be a positive time span when throttling is enabled.");

                RuleFor(options => options.MaxAlertsPerWindow)
                    .GreaterThanOrEqualTo(1).WithMessage("Max alerts per window must be at least 1 when throttling is enabled.");
            });
        }
    }

    /// <summary>
    /// Validator for the <see cref="DeduplicationOptions"/> configuration class.
    /// </summary>
    public class DeduplicationOptionsValidator : AbstractValidator<DeduplicationOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeduplicationOptionsValidator"/> class.
        /// </summary>
        public DeduplicationOptionsValidator()
        {
            When(options => options.IsEnabled, () =>
            {
                RuleFor(options => options.DeduplicationWindow)
                    .GreaterThan(TimeSpan.Zero).WithMessage("Deduplication window must be a positive time span when deduplication is enabled.");
            });
        }
    }
}