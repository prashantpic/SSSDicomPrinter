namespace TheSSS.DICOMViewer.Monitoring.Configuration;

public class ThrottlingOptions
{
    public bool IsEnabled { get; set; } = true;
    public TimeSpan DefaultThrottleWindow { get; set; } = TimeSpan.FromMinutes(30);
    public int MaxAlertsPerWindow { get; set; } = 3;
}