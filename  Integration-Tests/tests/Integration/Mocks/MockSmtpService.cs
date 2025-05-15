// Assuming interface is defined in TheSSS.DicomViewer.Application.Services
using TheSSS.DicomViewer.Application.Services; // For ISmtpService
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.IntegrationTests.Mocks
{
    public class SentEmailInfo
    {
        public string Recipient { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }

    public class MockSmtpService : ISmtpService
    {
        public List<SentEmailInfo> SentEmails { get; } = new List<SentEmailInfo>();

        public Task SendEmailAsync(string recipient, string subject, string body)
        {
            SentEmails.Add(new SentEmailInfo { Recipient = recipient, Subject = subject, Body = body });
            return Task.CompletedTask;
        }

        public void ClearSentEmails()
        {
            SentEmails.Clear();
        }
    }
}