using System.Windows;

namespace TheSSS.DicomViewer.Presentation.Services
{
    public class ThemeManager : IThemeManager
    {
        public void SetTheme(string themeName)
        {
            var dict = new ResourceDictionary
            {
                Source = new System.Uri($"/Resources/Themes/{themeName}.xaml", System.UriKind.Relative)
            };
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(dict);
        }

        public string GetCurrentTheme()
        {
            return "Light";
        }
    }
}