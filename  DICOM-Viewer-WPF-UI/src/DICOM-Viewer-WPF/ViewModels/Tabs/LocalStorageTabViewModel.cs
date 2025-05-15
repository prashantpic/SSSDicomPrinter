using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Presentation.ViewModels.Tabs
{
    public partial class LocalStorageTabViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<object> _localStudies = new();

        [ObservableProperty]
        private string _searchTerm = string.Empty;

        [ObservableProperty]
        private ThumbnailGridViewModel _thumbnailGridViewModel;

        [RelayCommand]
        private async Task SearchStudiesAsync()
        {
            await Task.Delay(100); // Simulate search delay
            // Update LocalStudies with filtered results
        }

        public LocalStorageTabViewModel()
        {
            _thumbnailGridViewModel = new ThumbnailGridViewModel();
            LocalStudies.Add(new { StudyId = "STD001", PatientName = "John Doe", StudyDate = "2024-03-01" });
        }
    }
}