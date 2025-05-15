namespace TheSSS.DICOMViewer.Common.Security
{
    public static class SecureStringExtensions
    {
        public static string ProtectString(this TheSSS.DICOMViewer.Common.Abstractions.Security.IDataProtectionProvider provider, string plaintext)
        {
            var bytes = Encoding.UTF8.GetBytes(plaintext);
            var protectedBytes = provider.ProtectData(bytes);
            return Convert.ToBase64String(protectedBytes);
        }

        public static string UnprotectString(this TheSSS.DICOMViewer.Common.Abstractions.Security.IDataProtectionProvider provider, string base64Ciphertext)
        {
            var protectedBytes = Convert.FromBase64String(base64Ciphertext);
            var bytes = provider.UnprotectData(protectedBytes);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}