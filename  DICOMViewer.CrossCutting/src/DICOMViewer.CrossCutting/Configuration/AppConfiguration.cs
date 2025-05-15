namespace TheSSS.DICOMViewer.Common.Configuration;

public class AppConfiguration : IAppConfiguration
{
    private readonly IConfiguration _configuration;
    private readonly IDataProtectionProvider _dataProtectionProvider;

    public AppConfiguration(IConfiguration configuration, IDataProtectionProvider dataProtectionProvider)
    {
        _configuration = configuration;
        _dataProtectionProvider = dataProtectionProvider;
    }

    public string? GetConnectionString(string name) => _configuration.GetConnectionString(name);

    public SmtpSettings? GetSmtpSettings()
    {
        var settings = _configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
        if (settings?.Password != null)
        {
            settings.Password = _dataProtectionProvider.UnprotectString(settings.Password);
        }
        return settings;
    }

    public OdooApiSettings? GetOdooApiSettings()
    {
        var settings = _configuration.GetSection("OdooApiSettings").Get<OdooApiSettings>();
        if (settings?.ApiKey != null)
        {
            settings.ApiKey = _dataProtectionProvider.UnprotectString(settings.ApiKey);
        }
        return settings;
    }

    public T? GetSection<T>(string key) => _configuration.GetSection(key).Get<T>();
    public T GetRequiredSection<T>(string key) => _configuration.GetRequiredSection(key).Get<T>() ?? throw new InvalidOperationException($"Configuration section '{key}' not found.");
}