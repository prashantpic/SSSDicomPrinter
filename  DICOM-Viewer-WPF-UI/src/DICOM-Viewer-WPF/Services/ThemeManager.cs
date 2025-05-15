using System.Windows;

namespace TheSSS.DicomViewer.Presentation.Services;

public class ThemeManager : IThemeManager
{
    public event EventHandler? ThemeChanged;

    public void SetTheme(string themeName)
    {
        var themeUri = new Uri($"/Resources/Themes/{themeName}.xaml", UriKind.Relative);
        var themeDict = new ResourceDictionary { Source = themeUri };
        
        Application.Current.Resources.MergedDictionaries.Clear();
        Application.Current.Resources.MergedDictionaries.Add(themeDict);
        
        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }

    public string GetCurrentTheme()
    {
        return "Light";
    }
}