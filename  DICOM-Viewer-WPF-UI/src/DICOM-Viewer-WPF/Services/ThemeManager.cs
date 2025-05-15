using System.Windows;

namespace TheSSS.DicomViewer.Presentation.Services
{
    public class ThemeManager : IThemeManager
    {
        private const string DefaultTheme = "Light";
        private string _currentTheme = DefaultTheme;

        public void SetTheme(string themeName)
        {
            if (_currentTheme == themeName) return;

            var resources = Application.Current.Resources;
            resources.MergedDictionaries.Clear();

            var themeUri =