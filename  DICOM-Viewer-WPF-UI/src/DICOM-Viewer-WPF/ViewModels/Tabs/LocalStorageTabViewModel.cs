using CommunityToolkit.Mvvm.ComponentModel;

namespace TheSSS.DicomViewer.Presentation.ViewModels.Tabs
{
    public partial class LocalStorageTabViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _storageStatus = "Local Storage Tab";
    }
}