using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using TheSSS.DICOMViewer.Presentation.Models;
using TheSSS.DICOMViewer.Presentation.Services;

namespace TheSSS.DICOMViewer.Presentation.ViewModels.Settings
{
    public partial class LocalizationSettingsPanelViewModel : ObservableObject
    {
        private readonly ILocalizationService _localizationService;

        [ObservableProperty]
        private LanguageItem? _selectedLanguage;

        public ObservableCollection<LanguageItem> AvailableLanguages { get; } = new();

        public LocalizationSettingsPanelViewModel(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
            InitializeLanguages();
        }

        private void InitializeLanguages()
        {
            AvailableLanguages.Clear();
            foreach (var lang in _localizationService.GetAvailableLanguages())
            {
                AvailableLanguages.Add(lang);
            }
            SelectedLanguage = AvailableLanguages[0];
        }
    }
}