using System.Security.Cryptography;
using System.Text;
using TheSSS.DICOMViewer.CrossCutting.Interfaces.Security;

namespace TheSSS.DICOMViewer.Infrastructure.Configuration.Security
{
    public class SecureConfigurationHandler : ISecureConfigurationHandler
    {
        public string Encrypt(string plainText)
        {
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var encryptedBytes = ProtectedData.Protect(plainBytes, null, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(encryptedBytes);
        }

        public string Decrypt(string encryptedText)
        {
            var encryptedBytes = Convert.FromBase64String(encryptedText);
            var decryptedBytes = ProtectedData.Unprotect(encryptedBytes, null, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(decryptedBytes);
        }
    }
}