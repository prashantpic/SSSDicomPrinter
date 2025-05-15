using System.Globalization;
using System.Resources;
using System.Threading;

namespace TheSSS.DicomViewer.Common.Localization;

public class ResourceManagerLocalizationManager : ILocalizationManager
{
    private readonly ResourceManager _resourceManager;

    public ResourceManagerLocalizationManager(Type resourceSourceAssemblyMarkerType, string baseName)
    {
        _resourceManager = new ResourceManager(baseName, resourceSourceAssemblyMarkerType.Assembly);
    }

    public CultureInfo CurrentCulture
    {
        get => Thread.CurrentThread.CurrentUICulture;
        set => Thread.CurrentThread.CurrentUICulture = value;
    }

    public string GetString(string key)
        => _resourceManager.GetString(key, CurrentCulture) ?? key;

    public string GetString(string key, CultureInfo culture)
        => _resourceManager.GetString(key, culture) ?? key;

    public IEnumerable<CultureInfo> GetAllSupportedCultures()
        => SupportedCultures.All;
}