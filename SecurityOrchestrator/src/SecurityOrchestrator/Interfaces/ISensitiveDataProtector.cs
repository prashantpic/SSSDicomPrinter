using System.Security.Cryptography;
using System.Threading.Tasks;

namespace TheSSS.DICOMViewer.Security.Interfaces;

/// <summary>
/// Defines the contract for encrypting and decrypting sensitive data,
/// abstracting the underlying protection mechanism (e.g., Windows Data Protection API - DPAPI).
/// Implemented in Infrastructure (e.g., using DPAPI wrapper).
/// Requirements Addressed: REQ-7-017.
/// </summary>
public interface ISensitiveDataProtector
{
    /// <summary>
    /// Protects (encrypts) the given plaintext data.
    /// </summary>
    /// <param name="plainData">The data to protect.</param>
    /// <param name="entropy">Optional additional entropy to increase security. Can be null.</param>
    /// <param name="scope">The scope of data protection (CurrentUser or LocalMachine).</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the protected (encrypted) data.</returns>
    Task<byte[]> ProtectAsync(byte[] plainData, string? entropy, DataProtectionScope scope);

    /// <summary>
    /// Unprotects (decrypts) the given protected data.
    /// </summary>
    /// <param name="protectedData">The data to unprotect.</param>
    /// <param name="entropy">Optional entropy that was used during protection. Must match the entropy used for protection if any. Can be null.</param>
    /// <param name="scope">The scope of data protection that was used (CurrentUser or LocalMachine).</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the unprotected (plaintext) data.</returns>
    Task<byte[]> UnprotectAsync(byte[] protectedData, string? entropy, DataProtectionScope scope);
}