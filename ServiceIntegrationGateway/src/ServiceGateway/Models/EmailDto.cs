using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object for sending emails.
    /// Contains recipient(s), sender, subject, body, and attachment details.
    /// </summary>
    public record EmailDto(
        string From,
        List<string> To,
        List<string>? Cc,
        List<string>? Bcc,
        string Subject,
        string? BodyHtml,
        string? BodyPlainText,
        List<AttachmentDto>? Attachments
    )
    {
        public EmailDto(string from, string to, string subject, string? bodyHtml = null, string? bodyPlainText = null)
            : this(from, new List<string> { to }, null, null, subject, bodyHtml, bodyPlainText, null)
        {
        }
    }
}