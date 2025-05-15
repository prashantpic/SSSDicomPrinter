using System;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using TheSSS.DICOMViewer.Integration.Configuration;
using TheSSS.DICOMViewer.Integration.Interfaces;
using TheSSS.DICOMViewer.Integration.Models;
using TheSSS.DICOMViewer.CrossCutting.Logging; // Assuming this is the namespace for ILoggerAdapter
using Polly;

namespace TheSSS.DICOMViewer.Integration.Adapters
{
    public class SmtpServiceAdapter : ISmtpServiceAdapter
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly ICredentialManager _credentialManager;
        private readonly IResiliencePolicyProvider _resiliencePolicyProvider;
        private readonly IUnifiedErrorHandlingService _errorHandlingService;
        private readonly ILoggerAdapter<SmtpServiceAdapter> _logger;
        // Optional: IRateLimiter if SMTP server has rate limits. Not explicitly in SmtpServiceAdapter description.

        private const string SmtpServiceIdentifier = "SMTPEmailService"; // Or from config

        public SmtpServiceAdapter(
            IOptions<SmtpSettings> smtpSettings,
            ICredentialManager credentialManager,
            IResiliencePolicyProvider resiliencePolicyProvider,
            IUnifiedErrorHandlingService errorHandlingService,
            ILoggerAdapter<SmtpServiceAdapter> logger)
        {
            _smtpSettings = smtpSettings.Value;
            _credentialManager = credentialManager;
            _resiliencePolicyProvider = resiliencePolicyProvider;
            _errorHandlingService = errorHandlingService;
            _logger = logger;
        }

        public async Task<SmtpSendResultDto> SendEmailAsync(EmailDto emailMessage, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to send email. Subject: {Subject}, To: {To}", emailMessage.Subject, string.Join(",", emailMessage.To));

            try
            {
                // Example: Apply a resilience policy for SMTP operations
                var policy = await _resiliencePolicyProvider.GetPolicyAsync(PolicyRegistryKeys.DefaultApiResiliencePolicy); // Or a specific SMTP policy

                return await policy.ExecuteAsync(async token =>
                {
                    using var smtpClient = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port);
                    smtpClient.EnableSsl = _smtpSettings.EnableSsl;

                    if (_smtpSettings.UseCredentials)
                    {
                        var credentials = await _credentialManager.GetCredentialsAsync(_smtpSettings.ServiceIdentifier ?? SmtpServiceIdentifier, token);
                        smtpClient.Credentials = new NetworkCredential(credentials.Username, credentials.Password);
                    }
                    
                    smtpClient.Timeout = _smtpSettings.TimeoutSeconds * 1000; // SmtpClient timeout is in milliseconds

                    using var mailMessage = new MailMessage();
                    mailMessage.From = new MailAddress(emailMessage.From ?? _smtpSettings.DefaultFromAddress);
                    
                    foreach (var toAddress in emailMessage.To) { mailMessage.To.Add(toAddress); }
                    if (emailMessage.Cc != null) { foreach (var ccAddress in emailMessage.Cc) { mailMessage.CC.Add(ccAddress); } }
                    if (emailMessage.Bcc != null) { foreach (var bccAddress in emailMessage.Bcc) { mailMessage.Bcc.Add(bccAddress); } }

                    mailMessage.Subject = emailMessage.Subject;
                    mailMessage.Body = emailMessage.BodyPlainText; // Assuming BodyPlainText for simplicity
                    mailMessage.IsBodyHtml = !string.IsNullOrEmpty(emailMessage.BodyHtml);
                    if (mailMessage.IsBodyHtml)
                    {
                        mailMessage.Body = emailMessage.BodyHtml;
                    }

                    // Handle attachments (EmailDto.Attachments -> System.Net.Mail.Attachment)
                    if (emailMessage.Attachments != null)
                    {
                        foreach (var attachmentDto in emailMessage.Attachments)
                        {
                            // Assuming AttachmentDto has Stream ContentStream and string FileName and string ContentType
                            // Need to handle stream disposal carefully if MailMessage doesn't take ownership.
                            // For safety, copy to a MemoryStream if the original stream might be disposed.
                            var memoryStream = new MemoryStream(); // Consider stream pooling for performance
                            await attachmentDto.ContentStream.CopyToAsync(memoryStream, token);
                            memoryStream.Position = 0;
                            mailMessage.Attachments.Add(new Attachment(memoryStream, attachmentDto.FileName, attachmentDto.ContentType));
                        }
                    }
                    
                    await smtpClient.SendMailAsync(mailMessage, token); 
                    
                    // SmtpClient.SendMailAsync(MailMessage, CancellationToken) is available from .NET 5
                    // For older targets or if SendMailAsync(MailMessage) is used, wrap in Task.Run for async.
                    // As this is .NET 8, SendMailAsync(mailMessage, token) is fine.

                    _logger.LogInformation("Email sent successfully. Subject: {Subject}", emailMessage.Subject);
                    return new SmtpSendResultDto(true, mailMessage.Headers["Message-ID"], "Sent successfully.");

                }, cancellationToken);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "SMTP error occurred while sending email. StatusCode: {StatusCode}", ex.StatusCode);
                var errorDto = _errorHandlingService.HandleError(ex, SmtpServiceIdentifier);
                return new SmtpSendResultDto(false, null, errorDto.Message, errorDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while sending email.");
                var errorDto = _errorHandlingService.HandleError(ex, SmtpServiceIdentifier);
                return new SmtpSendResultDto(false, null, errorDto.Message, errorDto);
            }
        }
    }
}