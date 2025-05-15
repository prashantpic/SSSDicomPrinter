using System;
using System.Collections.Generic;
using System.IO; // For Stream

namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Data Transfer Object for sending emails.
    /// Corresponds to REQ-5-011.
    /// </summary>
    public record EmailDto
    {
        /// <summary>
        /// The email address of the sender.
        /// </summary>
        public string FromAddress { get; init; }

        /// <summary>
        /// The display name of the sender. Optional.
        /// </summary>
        public string? FromDisplayName { get; init; }

        /// <summary>
        /// List of primary recipient email addresses.
        /// </summary>
        public List<string> ToAddresses { get; init; } = new List<string>();

        /// <summary>
        /// List of CC recipient email addresses. Optional.
        /// </summary>
        public List<string>? CcAddresses { get; init; }

        /// <summary>
        /// List of BCC recipient email addresses. Optional.
        /// </summary>
        public List<string>? BccAddresses { get; init; }

        /// <summary>
        /// The subject of the email.
        /// </summary>
        public string Subject { get; init; }

        /// <summary>
        /// The body content of the email.
        /// </summary>
        public string Body { get; init; }

        /// <summary>
        /// Indicates if the email body is HTML. If false, it's plain text.
        /// </summary>
        public bool IsHtml { get; init; }

        /// <summary>
        /// List of attachments for the email. Optional.
        /// </summary>
        public List<AttachmentDto>? Attachments { get; init; }

        public EmailDto(
            string fromAddress,
            List<string> toAddresses,
            string subject,
            string body,
            bool isHtml = false,
            string? fromDisplayName = null,
            List<string>? ccAddresses = null,
            List<string>? bccAddresses = null,
            List<AttachmentDto>? attachments = null)
        {
            if (string.IsNullOrWhiteSpace(fromAddress))
                throw new ArgumentException("From address cannot be null or whitespace.", nameof(fromAddress));
            if (toAddresses == null || toAddresses.Count == 0 || toAddresses.Any(string.IsNullOrWhiteSpace))
                throw new ArgumentException("To addresses list cannot be null, empty, or contain invalid addresses.", nameof(toAddresses));
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Subject cannot be null or whitespace.", nameof(subject));
            if (body == null) // Allow empty body
                throw new ArgumentNullException(nameof(body));

            FromAddress = fromAddress;
            FromDisplayName = fromDisplayName;
            ToAddresses = toAddresses;
            CcAddresses = ccAddresses;
            BccAddresses = bccAddresses;
            Subject = subject;
            Body = body;
            IsHtml = isHtml;
            Attachments = attachments;
        }
    }

    /// <summary>
    /// Data Transfer Object for an email attachment.
    /// </summary>
    public record AttachmentDto
    {
        /// <summary>
        /// The file name of the attachment as it should appear to the recipient.
        /// </summary>
        public string FileName { get; init; }

        /// <summary>
        /// The binary content of the attachment.
        /// </summary>
        public byte[] Content { get; init; }

        /// <summary>
        /// The MIME type of the attachment (e.g., "application/pdf", "image/jpeg").
        /// If null or empty, the mail client will attempt to determine it.
        /// </summary>
        public string? MimeType { get; init; }

        public AttachmentDto(string fileName, byte[] content, string? mimeType = null)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentException("File name cannot be null or whitespace.", nameof(fileName));
            if (content == null || content.Length == 0)
                throw new ArgumentException("Attachment content cannot be null or empty.", nameof(content));

            FileName = fileName;
            Content = content;
            MimeType = mimeType;
        }
    }
}