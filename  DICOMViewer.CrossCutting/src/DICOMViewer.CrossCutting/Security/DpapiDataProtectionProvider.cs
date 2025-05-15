namespace TheSSS.DICOMViewer.Common.Security;

public class DpapiDataProtectionProvider : IDataProtectionProvider
{
    public byte[] ProtectData(byte[] plaintext) => 
        ProtectedData.Protect(plaintext, null, DataProtectionScope.CurrentUser);

    public byte[] UnprotectData(byte[] ciphertext) => 
        ProtectedData.Unprotect(ciphertext, null, DataProtectionScope.CurrentUser);
}