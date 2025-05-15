using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

namespace TheSSS.DicomViewer.Presentation.Services
{
    public class NavigationService : INavigationService
    {
        private readonly Stack<object> _navigationStack = new();
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public bool CanGoBack => _navigationStack.Count > 0;

        public void NavigateTo<TViewModel>() where TViewModel : class
        {
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
            _navigationStack.Push(viewModel);
        }

        public void NavigateTo<TViewModel>(object parameter) where TViewModel : class
        {
            var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
            _navigationStack.Push(viewModel);
        }

        public void GoBack()
        {
            if (CanGoBack)
            {
                _navigationStack.Pop();
            }
        }
    }
}