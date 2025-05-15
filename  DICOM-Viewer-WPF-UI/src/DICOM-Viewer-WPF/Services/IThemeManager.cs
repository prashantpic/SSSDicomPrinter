namespace TheSSS.DicomViewer.Presentation.Services;

public interface IThemeManager
{
    void SetTheme(string themeName);
    string GetCurrentTheme();
    event EventHandler? ThemeChanged;
}