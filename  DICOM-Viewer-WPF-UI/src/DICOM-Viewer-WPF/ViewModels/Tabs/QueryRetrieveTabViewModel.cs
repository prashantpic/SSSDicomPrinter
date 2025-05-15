using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace TheSSS.DicomViewer.Presentation.ViewModels.Tabs
{
    public partial class QueryRetrieveTabViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _patientId = string.Empty;

        [ObservableProperty]
        private string _studyDate = string.Empty;

        [ObservableProperty]
        private ObservableCollection<object> _queryResults = new();

        [RelayCommand]
        private async Task ExecuteQueryAsync()
        {
            await Task.Delay(200); // Simulate query execution
            QueryResults.Add(new { PatientId = "PID001", StudyUID = "STD001" });
        }

        [RelayCommand]
        private async Task RetrieveSelectedStudiesAsync()
        {
            await Task.Delay(500); // Simulate retrieval
        }
    }
}