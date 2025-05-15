namespace TheSSS.DICOMViewer.Monitoring.Configuration;

public class AlertChannelSetting
{
    public string ChannelType { get; set; } = string.Empty;
    public bool IsEnabled { get; set; } = true;
    public List<string>? RecipientDetails { get; set; }
}