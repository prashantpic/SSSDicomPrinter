namespace TheSSS.DicomViewer.Common.Localization;

public interface ILocalizationManager
{
    System.Globalization.CultureInfo CurrentCulture { get; set; }
    string GetString(string key);
    string GetString(string key, System.Globalization.CultureInfo culture);
    System.Collections.Generic.IEnumerable<System.Globalization.CultureInfo> GetAllSupportedCultures();
}