namespace TheSSS.DICOMViewer.Monitoring.Interfaces.Adapters;

using System.Collections.Generic;
using System.Threading.Tasks;

public interface IEmailServiceAdapter
{
    Task SendEmailAsync(string recipient, string subject, string body);
    Task SendEmailAsync(IEnumerable<string> recipients, string subject, string body);
}