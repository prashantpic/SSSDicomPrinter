namespace TheSSS.DICOMViewer.Common.Configuration
{
    public class AppConfiguration : TheSSS.DICOMViewer.Common.Abstractions.Configuration.IAppConfiguration
    {
        private readonly IConfiguration _configuration;
        private readonly TheSSS.DICOMViewer.Common.Abstractions.Security.IDataProtectionProvider _dataProtectionProvider;

        public AppConfiguration(IConfiguration configuration, TheSSS.DICOMViewer.Common.Abstractions.Security.IDataProtectionProvider dataProtectionProvider)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _dataProtectionProvider = dataProtectionProvider ?? throw new ArgumentNullException(nameof(dataProtectionProvider));
        }

        public string GetConnectionString(string name) => _configuration.GetConnectionString(name) ?? throw new ArgumentNullException(name);

        public Models.SmtpSettings? GetSmtpSettings()
        {
            var settings = _configuration.GetSection("SmtpSettings").Get<Models.SmtpSettings>();
            if (settings?.Password != null)
            {
                settings.Password = _dataProtectionProvider.UnprotectString(settings.Password);
            }
            return settings;
        }

        public Models.OdooApiSettings? GetOdooApiSettings()
        {
            var settings = _configuration.GetSection("OdooApiSettings").Get<Models.OdooApiSettings>();
            if (settings?.ApiKey != null)
            {
                settings.ApiKey = _dataProtectionProvider.UnprotectString(settings.ApiKey);
            }
            return settings;
        }

        public T? GetSection<T>(string key) => _configuration.GetSection(key).Get<T>();
        public T GetRequiredSection<T>(string key) => _configuration.GetRequiredSection(key).Get<T>() ?? throw new InvalidOperationException($"Section {key} not found");
    }
}