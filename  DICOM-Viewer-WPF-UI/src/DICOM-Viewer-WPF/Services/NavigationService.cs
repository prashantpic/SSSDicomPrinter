using Microsoft.Extensions.DependencyInjection;
using System;

namespace TheSSS.DicomViewer.Presentation.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void NavigateTo(Type viewModelType)
        {
            var viewModel = _serviceProvider.GetRequiredService(viewModelType);
            // Implementation would depend on navigation framework
        }

        public void NavigateTo(Type viewModelType, object parameter)
        {
            throw new NotImplementedException();
        }

        public void GoBack()
        {
            throw new NotImplementedException();
        }
    }
}