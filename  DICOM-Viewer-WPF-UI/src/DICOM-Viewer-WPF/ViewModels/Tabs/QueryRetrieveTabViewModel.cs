using CommunityToolkit.Mvvm.ComponentModel;

namespace TheSSS.DicomViewer.Presentation.ViewModels.Tabs;

public partial class QueryRetrieveTabViewModel : ObservableObject
{
    [ObservableProperty]
    private string _pacsQuery = string.Empty;

    public QueryRetrieveTabViewModel()
    {
        // Initialize with service dependencies
    }
}