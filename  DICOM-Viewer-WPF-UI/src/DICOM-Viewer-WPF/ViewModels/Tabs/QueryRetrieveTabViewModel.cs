using CommunityToolkit.Mvvm.ComponentModel;

namespace TheSSS.DicomViewer.Presentation.ViewModels.Tabs
{
    public partial class QueryRetrieveTabViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _queryStatus = "Query/Retrieve Interface";
    }
}