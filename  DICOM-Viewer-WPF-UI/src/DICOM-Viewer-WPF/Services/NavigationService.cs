using Microsoft.Extensions.DependencyInjection;
using TheSSS.DicomViewer.Presentation.ViewModels;

namespace TheSSS.DicomViewer.Presentation.Services;

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly Stack<object> _navigationStack = new();

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public void NavigateTo<TViewModel>() where TViewModel : class
    {
        var viewModel = _serviceProvider.GetRequiredService<TViewModel>();
        _navigationStack.Push(viewModel);
    }

    public bool CanGoBack => _navigationStack.Count > 1;

    public void GoBack()
    {
        if (CanGoBack)
        {
            _navigationStack.Pop();
        }
    }
}