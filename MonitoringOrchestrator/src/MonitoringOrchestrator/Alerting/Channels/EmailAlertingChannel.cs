using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Exceptions;
using Microsoft.Extensions.Options;

namespace TheSSS.DICOMViewer.Monitoring.Alerting.Channels;

public class EmailAlertingChannel : IAlertingChannel
{
    private readonly IEmailServiceAdapter _emailService;
    private readonly AlertingOptions _options;
    public string ChannelType => "Email";

    public EmailAlertingChannel(
        IEmailServiceAdapter emailService,
        IOptions<AlertingOptions> options)
        => (_emailService, _options) = (emailService, options.Value);

    public async Task DispatchAlertAsync(NotificationPayloadDto payload, CancellationToken cancellationToken)
    {
        var channelConfig = _options.Channels.FirstOrDefault(c => c.ChannelType == ChannelType);
        if (channelConfig?.RecipientDetails == null || !channelConfig.IsEnabled)
            throw new AlertingSystemException(ChannelType, "Email channel not configured");

        try
        {
            await _emailService.SendEmailAsync(
                channelConfig.RecipientDetails,
                payload.Title,
                payload.Body);
        }
        catch (Exception ex)
        {
            throw new AlertingSystemException(ChannelType, "Email dispatch failed", payload, ex);
        }
    }
}