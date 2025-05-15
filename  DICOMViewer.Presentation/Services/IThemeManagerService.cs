using System.Collections.Generic;
using TheSSS.DICOMViewer.Presentation.Models;

namespace TheSSS.DICOMViewer.Presentation.Services
{
    public interface IThemeManagerService
    {
        IEnumerable<ThemeItem> GetAvailableThemes();
        Task ApplyThemeAsync(string themeName);
    }
}