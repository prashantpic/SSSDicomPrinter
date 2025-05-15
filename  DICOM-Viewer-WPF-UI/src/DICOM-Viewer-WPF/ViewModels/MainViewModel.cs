using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;

namespace TheSSS.DicomViewer.Presentation.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<object> _tabViewModels = new();

        [ObservableProperty]
        private object? _selectedTabViewModel;

        public MainViewModel(IServiceProvider serviceProvider)
        {
            TabViewModels.Add(serviceProvider.GetRequiredService<IncomingPrintQueueTabViewModel>());
            TabViewModels.Add(serviceProvider.GetRequiredService<LocalStorageTabViewModel>());
            TabViewModels.Add(serviceProvider.GetRequiredService<QueryRetrieveTabViewModel>());
            
            SelectedTabViewModel = TabViewModels.FirstOrDefault();
        }
    }
}