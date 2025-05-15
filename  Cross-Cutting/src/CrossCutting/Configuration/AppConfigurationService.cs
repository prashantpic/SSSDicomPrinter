using Microsoft.Extensions.Configuration;
using TheSSS.DicomViewer.Common.Configuration.Secure;

namespace TheSSS.DicomViewer.Common.Configuration;

public class AppConfigurationService : IAppConfigurationService
{
    private readonly IConfiguration _configuration;
    private readonly ISecureDataProtector _secureDataProtector;

    public AppConfigurationService(IConfiguration configuration, ISecureDataProtector secureDataProtector)
    {
        _configuration = configuration;
        _secureDataProtector = secureDataProtector;
    }

    public T GetValue<T>(string key) => _configuration.GetValue<T>(key);

    public IConfigurationSection GetSection(string key) => _configuration.GetSection(key);

    public void Bind<T>(string key, T instance) => _configuration.GetSection(key).Bind(instance);

    public string? GetProtectedValue(string key)
    {
        var protectedValue = _configuration[key];
        return string.IsNullOrEmpty(protectedValue) 
            ? null 
            : _secureDataProtector.UnprotectData(protectedValue);
    }
}