using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace TheSSS.DicomViewer.Common.Configuration.Secure;

public class DpapiDataProtector : ISecureDataProtector
{
    private readonly byte[]? _entropy;
    private readonly DataProtectionScope _scope;

    public DpapiDataProtector(IOptions<DpapiDataProtectorOptions> options)
    {
        var opts = options.Value;
        _scope = opts.Scope;
        _entropy = !string.IsNullOrEmpty(opts.EntropyBase64) 
            ? Convert.FromBase64String(opts.EntropyBase64) 
            : null;
    }

    public string ProtectData(string plainText)
    {
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var protectedBytes = ProtectedData.Protect(plainBytes, _entropy, _scope);
        return Convert.ToBase64String(protectedBytes);
    }

    public string UnprotectData(string protectedData)
    {
        try
        {
            var protectedBytes = Convert.FromBase64String(protectedData);
            var plainBytes = ProtectedData.Unprotect(protectedBytes, _entropy, _scope);
            return Encoding.UTF8.GetString(plainBytes);
        }
        catch (FormatException)
        {
            return string.Empty;
        }
        catch (CryptographicException)
        {
            return string.Empty;
        }
    }
}