using System;
using System.Net.Mail;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Polly;
using TheSSS.DICOMViewer.Integration.Interfaces;
using TheSSS.DICOMViewer.Integration.Models;
using TheSSS.DICOMViewer.Integration.Configuration;
using TheSSS.DICOMViewer.Common.Interfaces; // Assuming ILoggerAdapter is here
using System.IO;

namespace TheSSS.DICOMViewer.Integration.Adapters;

public class SmtpServiceAdapter : ISmtpServiceAdapter
{
    private readonly SmtpSettings _settings;
    private readonly IResiliencePolicyProvider _policyProvider;
    private readonly IRateLimiter _rateLimiter; // Placeholder for future rate limiting on SMTP
    private readonly ICredentialManager _credentialManager;
    private readonly ILoggerAdapter _logger;
    private readonly IAsyncPolicy _resiliencePolicy;
    private readonly ServiceGatewaySettings _gatewaySettings; // To check if SMTP is enabled

    public SmtpServiceAdapter(
        IOptions<SmtpSettings> settings,
        IOptions<ServiceGatewaySettings> gatewaySettings,
        IResiliencePolicyProvider policyProvider,
        IRateLimiter rateLimiter, // Injected for future use or consistency
        ICredentialManager credentialManager,
        ILoggerAdapter logger)
    {
        _settings = settings.Value ?? throw new ArgumentNullException(nameof(settings));
        _gatewaySettings = gatewaySettings.Value ?? throw new ArgumentNullException(nameof(gatewaySettings));
        _policyProvider = policyProvider ?? throw new ArgumentNullException(nameof(policyProvider));
        _rateLimiter = rateLimiter; // Store if needed
        _credentialManager = credentialManager ?? throw new ArgumentNullException(nameof(credentialManager));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Get the configured resilience policy for SMTP calls
        _resiliencePolicy = _policyProvider.GetPolicyAsync(_settings.PolicyKey);
    }

    public async Task<SmtpSendResultDto> SendEmailAsync(EmailDto emailMessage, CancellationToken cancellationToken = default)
    {
        if (!_gatewaySettings.EnableSmtpIntegration)
        {
            _logger.Information("SMTP integration is disabled. Skipping email send.");
            throw new ServiceIntegrationDisabledException("SMTP integration is disabled in settings.");
        }

        if (string.IsNullOrWhiteSpace(_settings.Server))
        {
            _logger.Error("SMTP server address is not configured. Cannot send email.");
            throw new InvalidOperationException("SMTP server address is not configured.");
        }

        // Consider applying rate limiting for SMTP if configured
        // if (_gatewaySettings.RateLimiting.EnableRateLimitingPerService && _rateLimiter != null)
        // {
        //    await _rateLimiter.AcquirePermitAsync("SmtpService", cancellationToken); // Use a specific key
        // }

        using var mailMessage = new MailMessage();
        mailMessage.From = new MailAddress(emailMessage.FromAddress, emailMessage.FromDisplayName);
        foreach (var to in emailMessage.ToRecipients) mailMessage.To.Add(to);
        if (emailMessage.CcRecipients != null) foreach (var cc in emailMessage.CcRecipients) mailMessage.CC.Add(cc);
        if (emailMessage.BccRecipients != null) foreach (var bcc in emailMessage.BccRecipients) mailMessage.Bcc.Add(bcc);
        mailMessage.Subject = emailMessage.Subject;
        mailMessage.Body = emailMessage.BodyHtml ?? emailMessage.BodyPlainText;
        mailMessage.IsBodyHtml = !string.IsNullOrWhiteSpace(emailMessage.BodyHtml);

        if (emailMessage.Attachments != null)
        {
            foreach (var attachmentDto in emailMessage.Attachments)
            {
                if (attachmentDto.Content != null && attachmentDto.Content.Length > 0)
                {
                    var stream = new MemoryStream(attachmentDto.Content);
                    // Ensure MemoryStream is not disposed before SmtpClient uses it.
                    // SmtpClient will dispose the stream after sending.
                    var attachment = new Attachment(stream, attachmentDto.FileName, attachmentDto.ContentType);
                    mailMessage.Attachments.Add(attachment);
                }
                 else
                 {
                     _logger.Warning($"Attachment '{attachmentDto.FileName}' has no content or zero length.");
                 }
            }
        }

        using var smtpClient = new SmtpClient(_settings.Server, _settings.Port)
        {
            EnableSsl = _settings.EnableSsl,
            UseDefaultCredentials = !_settings.RequiresAuthentication,
        };

        if (_settings.RequiresAuthentication)
        {
            ServiceCredentials credentials = await _credentialManager.GetCredentialsAsync(_settings.ServiceIdentifierForCredentials, cancellationToken);
            if (string.IsNullOrWhiteSpace(credentials.Username) || string.IsNullOrWhiteSpace(credentials.Password))
            {
                _logger.Error("SMTP authentication required but credentials are not configured or retrieved.");
                throw new CredentialRetrievalException($"Credentials for '{_settings.ServiceIdentifierForCredentials}' are required but not available.");
            }
            smtpClient.Credentials = new NetworkCredential(credentials.Username, credentials.Password);
        }

        try
        {
            // System.Net.Mail.SmtpClient.SendMailAsync does not accept CancellationToken directly.
            // Polly's timeout policy and cancellation will manage the outer operation.
            await _resiliencePolicy.ExecuteAsync(
                async ct => // ct here is Polly's CancellationToken, linked to the original if provided
                {
                    // If the operation itself is long-running and ct is signaled, SmtpClient won't abort.
                    // This is a limitation of System.Net.Mail.SmtpClient.
                    // Using MailKit library would provide better CancellationToken support.
                    await smtpClient.SendMailAsync(mailMessage); // Removed .ConfigureAwait(false) as SmtpClient doesn't really support it this way
                }, cancellationToken
            );

            _logger.Information($"Email sent successfully to {string.Join(",", emailMessage.ToRecipients)}.");
            return new SmtpSendResultDto(true, "Email sent successfully.", null); // MessageId not typically returned by System.Net.Mail
        }
        catch (SmtpException smtpEx)
        {
            _logger.Error(smtpEx, $"SMTP error sending email to {string.Join(",", emailMessage.ToRecipients)}. Status Code: {smtpEx.StatusCode}");
            throw new SmtpSendException($"Failed to send email via SMTP. Status Code: {smtpEx.StatusCode}", smtpEx);
        }
        catch (OperationCanceledException opCancelEx) when (cancellationToken.IsCancellationRequested)
        {
             _logger.Warning(opCancelEx, $"Email sending was cancelled for recipients: {string.Join(",", emailMessage.ToRecipients)}.");
             throw new SmtpSendException("Email sending operation was cancelled.", opCancelEx);
        }
        catch (Exception ex)
        {
            _logger.Error(ex, $"An unexpected error occurred while sending email to {string.Join(",", emailMessage.ToRecipients)}.");
            throw new SmtpSendException("An unexpected error occurred during email sending.", ex);
        }
        // Note: SmtpClient and MailMessage are IDisposable. `using` statement handles their disposal.
        // Attachments using MemoryStream will be disposed by MailMessage.Dispose().
    }
}

// Custom exception for SMTP errors
public class SmtpSendException : Exception
{
    public SmtpSendException(string message, Exception innerException) : base(message, innerException) { }
}

// Custom exception for credential retrieval errors
public class CredentialRetrievalException : Exception
{
    public CredentialRetrievalException(string message) : base(message) { }
    public CredentialRetrievalException(string message, Exception innerException) : base(message, innerException) { }
}