namespace TheSSS.DICOMViewer.Monitoring.Configuration;

using System;

public class MonitoringOptions
{
    public const string SectionName = "Monitoring";
    public bool IsMonitoringEnabled { get; set; } = true;
    public TimeSpan SystemHealthCheckInterval { get; set; } = TimeSpan.FromMinutes(5);
    public TimeSpan CriticalErrorLookbackPeriod { get; set; } = TimeSpan.FromHours(24);
}