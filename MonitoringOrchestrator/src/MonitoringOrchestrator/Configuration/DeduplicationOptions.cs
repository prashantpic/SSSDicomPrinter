namespace TheSSS.DICOMViewer.Monitoring.Configuration;

public class DeduplicationOptions
{
    public bool IsEnabled { get; set; } = true;
    public TimeSpan DeduplicationWindow { get; set; } = TimeSpan.FromMinutes(15);
}