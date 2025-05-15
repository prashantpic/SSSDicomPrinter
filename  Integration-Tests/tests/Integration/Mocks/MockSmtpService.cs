using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TheSSS.DicomViewer.Application; // Assuming ISmtpService is here

namespace TheSSS.DicomViewer.IntegrationTests.Mocks
{
    public class SentEmailInfo
    {
        public string Recipient { get; }
        public string Subject { get; }
        public string Body { get; }

        public SentEmailInfo(string recipient, string subject, string body)
        {
            Recipient = recipient;
            Subject = subject;
            Body = body;
        }
    }

    // Minimal placeholder for ISmtpService (assuming it's in TheSSS.DicomViewer.Application)
    /*
    namespace TheSSS.DicomViewer.Application
    {
        public interface ISmtpService
        {
            Task SendEmailAsync(string recipient, string subject, string body, CancellationToken cancellationToken = default);
        }
    }
    */

    public class MockSmtpService : ISmtpService
    {
        public List<SentEmailInfo> SentEmails { get; } = new List<SentEmailInfo>();

        public Task SendEmailAsync(string recipient, string subject, string body, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            SentEmails.Add(new SentEmailInfo(recipient, subject, body));
            return Task.CompletedTask;
        }

        public void Clear()
        {
            SentEmails.Clear();
        }
    }
}