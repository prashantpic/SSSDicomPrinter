namespace TheSSS.DICOMViewer.Common.Abstractions.Configuration;

public interface IAppConfiguration
{
    string? GetConnectionString(string name);
    SmtpSettings? GetSmtpSettings();
    OdooApiSettings? GetOdooApiSettings();
    T? GetSection<T>(string key);
    T GetRequiredSection<T>(string key);
}