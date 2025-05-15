using TheSSS.DICOMViewer.Monitoring.Interfaces;
using TheSSS.DICOMViewer.Monitoring.Contracts;
using TheSSS.DICOMViewer.Monitoring.Exceptions;

namespace TheSSS.DICOMViewer.Monitoring.Alerting.Channels;

public class UiNotificationChannel : IAlertingChannel
{
    private readonly IUiNotificationAdapter _uiAdapter;
    public string ChannelType => "UI";

    public UiNotificationChannel(IUiNotificationAdapter uiAdapter) 
        => _uiAdapter = uiAdapter;

    public async Task DispatchAlertAsync(NotificationPayloadDto payload, CancellationToken cancellationToken)
    {
        try
        {
            await _uiAdapter.SendUi