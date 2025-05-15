using System.Security.Cryptography;

namespace TheSSS.DICOMViewer.Security.DTOs
{
    /// <summary>
    /// Data transfer object carrying data and parameters for an encryption/protection request.
    /// REQ-7-017
    /// </summary>
    /// <param name="DataToProtect">The byte array containing the data to be protected.</param>
    /// <param name="Entropy">Optional additional entropy (e.g., a salt) to increase security.</param>
    /// <param name="Scope">The scope of data protection (CurrentUser or LocalMachine).</param>
    public record SensitiveDataProtectionRequestDto(
        byte[] DataToProtect,
        string? Entropy,
        DataProtectionScope Scope);
}