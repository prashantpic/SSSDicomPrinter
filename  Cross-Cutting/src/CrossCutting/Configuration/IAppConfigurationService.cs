namespace TheSSS.DicomViewer.Common.Configuration;

public interface IAppConfigurationService
{
    T GetValue<T>(string key);
    Microsoft.Extensions.Configuration.IConfigurationSection GetSection(string key);
    void Bind<T>(string key, T instance);
    string? GetProtectedValue(string key);
}