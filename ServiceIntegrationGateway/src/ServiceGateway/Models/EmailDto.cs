using System.Collections.Generic;

namespace TheSSS.DICOMViewer.Integration.Models;

/// <summary>
/// Data Transfer Object for sending an email message.
/// Contains all necessary information such as recipients, sender, subject, body, and attachments.
/// </summary>
/// <param name="FromAddress">The sender's email address.</param>
/// <param name="FromDisplayName">An optional display name for the sender.</param>
/// <param name="ToRecipients">A list of primary recipients' email addresses.</param>
/// <param name="CcRecipients">An optional list of CC recipients' email addresses.</param>
/// <param name="BccRecipients">An optional list of BCC recipients' email addresses.</param>
/// <param name="Subject">The subject line of the email.</param>
/// <param name="BodyPlainText">The plain text version of the email body. One of BodyPlainText or BodyHtml should be provided.</param>
/// <param name="BodyHtml">The HTML version of the email body. If provided, IsBodyHtml should be true.</param>
/// <param name="Attachments">An optional list of attachments for the email.</param>
public record EmailDto(
    string FromAddress,
    string? FromDisplayName,
    List<string> ToRecipients,
    List<string>? CcRecipients,
    List<string>? BccRecipients,
    string Subject,
    string? BodyPlainText,
    string? BodyHtml,
    List<EmailAttachmentDto>? Attachments
)
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailDto"/> class with minimal required fields.
    /// </summary>
    public EmailDto(string fromAddress, List<string> toRecipients, string subject, string bodyPlainText)
        : this(fromAddress, null, toRecipients, null, null, subject, bodyPlainText, null, null) { }
}

/// <summary>
/// Data Transfer Object representing an email attachment.
/// </summary>
/// <param name="FileName">The name of the attachment file (e.g., "document.pdf").</param>
/// <param name="ContentType">The MIME type of the attachment content (e.g., "application/pdf", "image/jpeg").</param>
/// <param name="Content">The binary content of the attachment as a byte array.</param>
public record EmailAttachmentDto(
    string FileName,
    string ContentType,
    byte[] Content
);