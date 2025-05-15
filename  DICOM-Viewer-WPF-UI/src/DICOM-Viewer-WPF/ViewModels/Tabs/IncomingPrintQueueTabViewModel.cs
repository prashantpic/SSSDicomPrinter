using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Presentation.ViewModels.Tabs
{
    public partial class IncomingPrintQueueTabViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<object> _printQueueItems = new();

        [ObservableProperty]
        private bool _isProcessing;

        [RelayCommand]
        private async Task ProcessSelectedItemsAsync()
        {
            IsProcessing = true;
            await Task.Delay(1000); // Simulate processing
            IsProcessing = false;
        }

        public IncomingPrintQueueTabViewModel()
        {
            // Initialize with sample data
            PrintQueueItems.Add(new { StudyId = "STD001", PatientName = "John Doe", Status = "Pending" });
        }
    }
}