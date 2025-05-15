using System.Security.Cryptography;

namespace TheSSS.DICOMViewer.Security.DTOs
{
    /// <summary>
    /// Data transfer object carrying data and parameters for a decryption/unprotection request.
    /// REQ-7-017
    /// </summary>
    /// <param name="ProtectedData">The byte array containing the data to be unprotected.</param>
    /// <param name="Entropy">Optional entropy that was used during protection.</param>
    /// <param name="Scope">The scope of data protection that was used (CurrentUser or LocalMachine).</param>
    public record SensitiveDataUnprotectionRequestDto(
        byte[] ProtectedData,
        string? Entropy,
        DataProtectionScope Scope);
}