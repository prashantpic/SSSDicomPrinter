namespace TheSSS.DICOMViewer.Monitoring.Configuration;

using System;

public class ThrottlingOptions
{
    public bool IsEnabled { get; set; } = true;
    public TimeSpan DefaultThrottleWindow { get; set; } = TimeSpan.FromHours(1);
    public int MaxAlertsPerWindow { get; set; } = 3;
}