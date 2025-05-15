using Microsoft.Extensions.DependencyInjection;

namespace TheSSS.DicomViewer.Presentation.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public void NavigateTo<TViewModel>() where TViewModel : class
        {
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
        }

        public void NavigateTo<TViewModel>(object parameter) where TViewModel : class
        {
        }

        public void GoBack()
        {
        }
    }
}