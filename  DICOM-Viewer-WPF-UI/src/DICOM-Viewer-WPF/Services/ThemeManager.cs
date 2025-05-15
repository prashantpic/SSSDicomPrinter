using System.Windows;
using TheSSS.DicomViewer.Presentation.Properties;

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
            
            resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = themeName switch
                {
                    "Dark" => new System.Uri("/TheSSS.DicomViewer.Presentation;component/Resources/Themes/DarkTheme.xaml", 
                        System.UriKind.Relative),
                    _ => new System.Uri("/TheSSS.DicomViewer.Presentation;component/Resources/Themes/LightTheme.xaml", 
                        System.UriKind.Relative)
                }
            });

            _currentTheme = themeName;
        }

        public string GetCurrentTheme() => _currentTheme;
    }
}