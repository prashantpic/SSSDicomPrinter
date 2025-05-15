namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

public interface IEmailServiceAdapter
{
    Task SendEmailAsync(string recipient, string subject, string body);
    Task SendEmailAsync(IEnumerable<string> recipients, string subject, string body);
}