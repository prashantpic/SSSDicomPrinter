namespace TheSSS.DICOMViewer.Monitoring.Configuration;

using System.Collections.Generic;

public class AlertChannelSetting
{
    public string ChannelType { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public List<string>? RecipientDetails { get; set; }
    public List<string>? Severities { get; set; }
}