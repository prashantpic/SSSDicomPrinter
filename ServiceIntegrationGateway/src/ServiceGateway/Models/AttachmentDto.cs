namespace TheSSS.DICOMViewer.Integration.Models
{
    /// <summary>
    /// Represents an email attachment.
    /// </summary>
    public record AttachmentDto(
        string FileName,
        byte[] Content, // Could also be Stream, but byte[] is simpler for DTOs
        string ContentType // MIME type, e.g., "application/pdf", "image/jpeg"
    );
}