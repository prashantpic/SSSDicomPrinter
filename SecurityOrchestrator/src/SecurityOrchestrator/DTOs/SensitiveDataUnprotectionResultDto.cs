namespace TheSSS.DICOMViewer.Security.DTOs
{
    /// <summary>
    /// Data transfer object representing the result of a data unprotection operation.
    /// REQ-7-017
    /// </summary>
    /// <param name="UnprotectedData">The byte array containing the unprotected (decrypted) data.</param>
    public record SensitiveDataUnprotectionResultDto(
        byte[] UnprotectedData);
}