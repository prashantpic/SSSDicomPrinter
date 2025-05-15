namespace TheSSS.DICOMViewer.Common.Abstractions.Security;

public interface IDataProtectionProvider
{
    byte[] ProtectData(byte[] plaintext);
    byte[] UnprotectData(byte[] ciphertext);
}