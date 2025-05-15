using System.Collections.Generic;
using System.Threading.Tasks;
using TheSSS.DICOMViewer.Presentation.Models;

namespace TheSSS.DICOMViewer.Presentation.Services
{
    public class ThemeManagerService : IThemeManagerService
    {
        private readonly IAppSettingsService _appSettingsService;

        public ThemeManagerService(IAppSettingsService appSettingsService)
        {
            _appSettingsService = appSettingsService;
        }

        public IEnumerable<ThemeItem> GetAvailableThemes()
        {
            return new List<ThemeItem>
            {
                new() { DisplayName = "Light", Name = "Light" },
                new() { DisplayName = "Dark", Name = "Dark" },
                new() { DisplayName = "High Contrast", Name = "HighContrast" }
            };
        }

        public async Task ApplyThemeAsync(string themeName)
        {
            // Implementation for applying theme
        }
    }
}