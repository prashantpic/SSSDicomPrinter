namespace TheSSS.DicomViewer.Common.Configuration.Secure;

public class DpapiDataProtectorOptions
{
    public string? EntropyBase64 { get; set; }
    public System.Security.Cryptography.DataProtectionScope Scope { get; set; } = 
        System.Security.Cryptography.DataProtectionScope.CurrentUser;
}