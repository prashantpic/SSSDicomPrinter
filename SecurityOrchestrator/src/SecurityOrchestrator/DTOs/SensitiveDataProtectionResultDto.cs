namespace TheSSS.DICOMViewer.Security.DTOs
{
    /// <summary>
    /// Data transfer object representing the result of a data protection operation.
    /// REQ-7-017
    /// </summary>
    /// <param name="ProtectedData">The byte array containing the protected (encrypted) data.</param>
    public record SensitiveDataProtectionResultDto(
        byte[] ProtectedData);
}