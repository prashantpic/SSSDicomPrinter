namespace TheSSS.DICOMViewer.Common.Abstractions.Configuration
{
    public interface IAppConfiguration
    {
        string GetConnectionString(string name);
        TheSSS.DICOMViewer.Common.Configuration.Models.SmtpSettings? GetSmtpSettings();
        TheSSS.DICOMViewer.Common.Configuration.Models.OdooApiSettings? GetOdooApiSettings();
        T? GetSection<T>(string key);
        T GetRequiredSection<T>(string key);
    }
}