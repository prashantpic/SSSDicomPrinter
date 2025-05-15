namespace TheSSS.DicomViewer.Common.Logging.Serilog.Phi;

public class PhiScrubberOptions
{
    public System.Collections.Generic.List<string> Patterns { get; set; } = new();
    public string Replacement { get; set; } = "[PHI_REDACTED]";
}