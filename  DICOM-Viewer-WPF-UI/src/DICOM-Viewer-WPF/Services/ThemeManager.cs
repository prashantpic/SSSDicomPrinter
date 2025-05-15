using System.Windows;
using System.Linq;

namespace TheSSS.DicomViewer.Presentation.Services
{
    public class ThemeManager : IThemeManager
    {
        public void SetTheme(string themeName)
        {
            var themeDict = new ResourceDictionary
            {
                Source = new Uri($"/TheSSS.DicomViewer.Presentation;component/Resources/Themes/{themeName}Theme.xaml", UriKind.Relative)
            };

            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(themeDict);
        }

        public string GetCurrentTheme()
        {
            return Application.Current.Resources.MergedDictionaries
                .FirstOrDefault()?.Source?.ToString()?.Split('/').Last()?.Split('T').First() ?? "Light";
        }
    }
}