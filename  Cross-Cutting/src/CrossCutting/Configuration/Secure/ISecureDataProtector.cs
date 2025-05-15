namespace TheSSS.DicomViewer.Common.Configuration.Secure;

public interface ISecureDataProtector
{
    string ProtectData(string plainText);
    string UnprotectData(string protectedData);
}