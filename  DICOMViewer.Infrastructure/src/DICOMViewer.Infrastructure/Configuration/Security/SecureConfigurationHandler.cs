using System.Security.Cryptography;
using TheSSS.DICOMViewer.CrossCutting.Logging;

namespace TheSSS.DICOMViewer.Infrastructure.Configuration.Security
{
    public class SecureConfigurationHandler : ISecureConfigurationHandler
    {
        private readonly ILoggerAdapter<SecureConfigurationHandler> _logger;

        public SecureConfigurationHandler(ILoggerAdapter<SecureConfigurationHandler> logger)
        {
            _logger = logger;
        }

        public string Encrypt(string plainText)
        {
            try
            {
                var encryptedData = ProtectedData.Protect(
                    System.Text.Encoding.UTF8.GetBytes(plainText),
                    null,
                    DataProtectionScope.CurrentUser);
                
                return Convert.ToBase64String(encryptedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Encryption failed");
                return null;
            }
        }

        public string Decrypt(string cipherText)
        {
            try
            {
                var decryptedData = ProtectedData.Unprotect(
                    Convert.FromBase64String(cipherText),
                    null,
                    DataProtectionScope.CurrentUser);
                
                return System.Text.Encoding.UTF8.GetString(decryptedData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Decryption failed");
                return null;
            }
        }
    }
}