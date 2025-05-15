using System;
using Microsoft.Extensions.DependencyInjection;

namespace TheSSS.DICOMViewer.Presentation.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly MainApplicationWindowViewModel _mainAppViewModel;

        public NavigationService(
            IServiceProvider serviceProvider,
            MainApplicationWindowViewModel mainAppViewModel)
        {
            _serviceProvider = serviceProvider;
            _mainAppViewModel = mainAppViewModel;
        }

        public void NavigateTo(Type viewModelType)
        {
            var viewModel = _serviceProvider.GetRequiredService(viewModelType);
            _mainAppViewModel.CurrentMainViewModel = (ObservableObject)viewModel;
        }
    }
}